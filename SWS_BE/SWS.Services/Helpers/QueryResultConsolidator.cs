using System.Dynamic;
using SWS.Services.ApiModels.SqlConverts;
using System.Text.Json;

namespace SWS.Services.Helpers
{
    /// <summary>
    /// Helper để tổng hợp và format kết quả từ multiple queries hoặc structured JSON response thành dạng đẹp cho FE
    /// </summary>
    public class QueryResultConsolidator
    {
        // ===== HELPER METHODS (di chuyển lên đầu để tránh lỗi scope) =====
        
        /// <summary>
        /// Get list of column names from a dictionary (null-safe)
        /// </summary>
        private static List<string> GetColumnNames(Dictionary<string, object>? dict)
        {
            if (dict == null) return new List<string>();
            return dict.Keys.ToList();
        }

        /// <summary>
        /// Get a value from dictionary with multiple candidate keys (case-insensitive)
        /// </summary>
        private static object? GetFirstExistingValue(Dictionary<string, object> dict, params string[] candidates)
        {
            foreach (var cand in candidates)
            {
                if (dict.TryGetValue(cand, out var v)) return v;
                var kv = dict.FirstOrDefault(k => string.Equals(k.Key, cand, StringComparison.OrdinalIgnoreCase));
                if (!kv.Equals(default(KeyValuePair<string, object>))) return kv.Value;
            }
            // fallback: try keys that contain candidate tokens
            foreach (var cand in candidates)
            {
                var found = dict.FirstOrDefault(k => k.Key.IndexOf(cand, StringComparison.OrdinalIgnoreCase) >= 0);
                if (!found.Equals(default(KeyValuePair<string, object>))) return found.Value;
            }
            return null;
        }

        /// <summary>
        /// Try to parse an integer-like value from dict by several candidate keys
        /// </summary>
        private static int GetIntFromCandidates(Dictionary<string, object> dict, params string[] candidates)
        {
            foreach (var cand in candidates)
            {
                var val = GetFirstExistingValue(dict, cand);
                if (val == null || val is DBNull) continue;
                try
                {
                    if (val is long l) return (int)l;
                    if (val is int i) return i;
                    if (val is double d) return (int)d;
                    if (val is decimal dec) return (int)dec;
                    if (int.TryParse(val.ToString(), out var parsed)) return parsed;
                    if (double.TryParse(val.ToString(), out var dp)) return (int)dp;
                }
                catch { }
            }
            return 0;
        }

        /// <summary>
        /// Get string from candidates
        /// </summary>
        private static string? GetStringFromCandidates(Dictionary<string, object> dict, params string[] candidates)
        {
            var v = GetFirstExistingValue(dict, candidates);
            if (v == null || v is DBNull) return null;
            return v.ToString();
        }

        /// <summary>
        /// Backwards-compatible GetValue helper used by older code paths. Tries case-insensitive lookup.
        /// </summary>
        private static object? GetValue(Dictionary<string, object> dict, string key)
        {
            if (dict == null) return null;
            if (dict.TryGetValue(key, out var v)) return v;
            var kv = dict.FirstOrDefault(k => string.Equals(k.Key, key, StringComparison.OrdinalIgnoreCase));
            if (!kv.Equals(default(KeyValuePair<string, object>))) return kv.Value;
            return null;
        }

        // ===== MAIN CONSOLIDATION METHODS =====

        /// <summary>
        /// Tổng hợp multiple query results thành format đẹp, merge data liên quan
        /// </summary>
        public static ConsolidatedQueryResultDto Consolidate(MultiSqlQueryResultDto multiResult, string searchKeyword)
        {
            var consolidated = new ConsolidatedQueryResultDto
            {
                SearchKeyword = searchKeyword,
                TotalQueriesExecuted = multiResult.Results.Count,
                OriginalQueries = multiResult.Queries,
                Results = new List<SmartQueryResultDto>()
            };

            // Phân loại và xử lý từng loại result
            var productDetails = multiResult.Results.FirstOrDefault(r => 
                r.Query.Contains("ProductID") && r.Query.Contains("FROM Product") && 
                !r.Query.Contains("SUM") && !r.Query.Contains("GROUP BY"));

            var inventorySummary = multiResult.Results.FirstOrDefault(r => 
                r.Query.Contains("SUM") && r.Query.Contains("QuantityAvailable"));

            var locationInfo = multiResult.Results.FirstOrDefault(r => 
                r.Query.Contains("ShelfID") && r.Query.Contains("Location"));

            // 1. Xử lý Product Details (nếu có)
            if (productDetails != null && productDetails.RowCount > 0)
            {
                var productResult = ProcessProductDetails(productDetails);
                if (productResult != null)
                    consolidated.Results.Add(productResult);
            }

            // 2. Xử lý Inventory Summary kết hợp với Location
            if (inventorySummary != null || locationInfo != null)
            {
                var combinedResult = MergeInventoryAndLocation(inventorySummary, locationInfo, searchKeyword);
                if (combinedResult != null)
                    consolidated.Results.Add(combinedResult);
            }

            // 3. Tạo Overall Summary
            consolidated.OverallSummary = CreateOverallSummary(multiResult.Results, searchKeyword);

            return consolidated;
        }

        private static SmartQueryResultDto? ProcessProductDetails(QueryResultSet productResult)
        {
            try
            {
                var dataList = ConvertToListDict(productResult.Data);
                if (dataList == null || dataList.Count == 0) return null;

                return new SmartQueryResultDto
                {
                    QueryType = "product_details",
                    Title = "Thông tin sản phẩm",
                    TotalRecords = dataList.Count,
                    Data = dataList,
                    Metadata = new Dictionary<string, object>
                    {
                        { "source", "Product table" },
                        { "columns", GetColumnNames(dataList.FirstOrDefault()) }
                    }
                };
            }
            catch
            {
                return null;
            }
        }

        private static SmartQueryResultDto? MergeInventoryAndLocation(
            QueryResultSet? inventoryResult, 
            QueryResultSet? locationResult,
            string searchKeyword)
        {
            try
            {
                var inventoryData = inventoryResult != null ? ConvertToListDict(inventoryResult.Data) : null;
                var locationData = locationResult != null ? ConvertToListDict(locationResult.Data) : null;

                if (inventoryData == null && locationData == null) return null;

                // Merge data theo ProductName
                var mergedData = new List<Dictionary<string, object>>();
                var summary = new Dictionary<string, object>();

                if (inventoryData != null && inventoryData.Count > 0)
                {
                    foreach (var invItem in inventoryData)
                    {
                        // Product name may be under many aliases: ProductName, Name
                        var productName = GetStringFromCandidates(invItem, "ProductName", "Name", "name") ?? "Unknown";
                        var totalAvailable = GetIntFromCandidates(invItem, "TotalAvailableQuantity", "TotalAvailable", "TotalQuantityAvailable", "QuantityAvailable", "SumQuantity") ;
                        var totalAllocated = GetIntFromCandidates(invItem, "TotalAllocatedQuantity", "TotalAllocated", "AllocatedQuantity");

                        // Tìm location info tương ứng
                        var locationItems = locationData?.Where(l =>
                            (GetStringFromCandidates(l, "ProductName", "Name") ?? string.Empty) == productName
                        ).ToList() ?? new List<Dictionary<string, object>>();

                        var mergedItem = new Dictionary<string, object>
                        {
                            { "productName", productName },
                            { "totalAvailable", totalAvailable },
                            { "totalAllocated", totalAllocated },
                            { "totalInStock", totalAvailable + totalAllocated },
                            { "locations", locationItems.Select(loc => new Dictionary<string, object>
                            {
                                { "shelf", GetStringFromCandidates(loc, "ShelfID", "Shelf") ?? "" },
                                { "column", GetIntFromCandidates(loc, "ColumnNumber", "Column") },
                                { "row", GetIntFromCandidates(loc, "RowNumber", "Row") },
                                { "type", GetStringFromCandidates(loc, "LocationType", "Type") ?? "" },
                                { "quantity", GetIntFromCandidates(loc, "QuantityAvailable", "Quantity", "TotalQuantityAvailable") }
                            }).ToList() }
                        };

                        mergedData.Add(mergedItem);

                        // Cập nhật summary
                        if (summary.Count == 0)
                        {
                            summary["totalProducts"] = 0;
                            summary["totalQuantityAvailable"] = 0;
                            summary["totalQuantityAllocated"] = 0;
                            summary["totalLocations"] = 0;
                        }

                        summary["totalProducts"] = (int)summary["totalProducts"] + 1;
                        summary["totalQuantityAvailable"] = (int)summary["totalQuantityAvailable"] + totalAvailable;
                        summary["totalQuantityAllocated"] = (int)summary["totalQuantityAllocated"] + totalAllocated;
                        summary["totalLocations"] = (int)summary["totalLocations"] + locationItems.Count;
                    }
                }
                else if (locationData != null && locationData.Count > 0)
                {
                    // Chỉ có location data, group theo product
                    var groupedByProduct = locationData.GroupBy(l => GetStringFromCandidates(l, "ProductName", "Name") ?? "Unknown");
                    
                    foreach (var group in groupedByProduct)
                    {
                        var locations = group.Select(loc => new Dictionary<string, object>
                        {
                            { "shelf", GetStringFromCandidates(loc, "ShelfID", "Shelf") ?? "" },
                            { "column", GetIntFromCandidates(loc, "ColumnNumber", "Column") },
                            { "row", GetIntFromCandidates(loc, "RowNumber", "Row") },
                            { "type", GetStringFromCandidates(loc, "LocationType", "Type") ?? "" },
                            { "quantity", GetIntFromCandidates(loc, "QuantityAvailable", "Quantity", "TotalQuantityAvailable") }
                        }).ToList();

                        var totalQty = locations.Sum(l => Convert.ToInt32(l["quantity"]));

                        mergedData.Add(new Dictionary<string, object>
                        {
                            { "productName", group.Key },
                            { "totalAvailable", totalQty },
                            { "totalAllocated", 0 },
                            { "totalInStock", totalQty },
                            { "locations", locations }
                        });
                    }
                }

                return new SmartQueryResultDto
                {
                    QueryType = "inventory_location",
                    Title = $"Tồn kho và vị trí - {searchKeyword}",
                    TotalRecords = mergedData.Count,
                    Data = mergedData,
                    Summary = summary.Count > 0 ? summary : null,
                    Metadata = new Dictionary<string, object>
                    {
                        { "source", "Inventory + Location tables" },
                        { "searchKeyword", searchKeyword }
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to merge inventory and location: {ex.Message}");
                return null;
            }
        }

        private static Dictionary<string, object> CreateOverallSummary(List<QueryResultSet> results, string searchKeyword)
        {
            var summary = new Dictionary<string, object>
            {
                { "searchKeyword", searchKeyword },
                { "totalQueriesExecuted", results.Count },
                { "totalRecordsReturned", results.Sum(r => r.RowCount) }
            };

            // Tìm kiếm thông tin về product
            var productResult = results.FirstOrDefault(r => r.TableName.Contains("Product") && r.RowCount > 0);
            if (productResult != null)
            {
                summary["productsFound"] = productResult.RowCount;
            }

            // Tìm kiếm thông tin inventory
            var inventoryResult = results.FirstOrDefault(r => 
                r.Query.Contains("QuantityAvailable") || r.Query.Contains("Inventory"));
            if (inventoryResult != null)
            {
                var invData = ConvertToListDict(inventoryResult.Data);
                if (invData != null && invData.Count > 0)
                {
                    var firstItem = invData.FirstOrDefault();
                    if (firstItem != null && firstItem.ContainsKey("TotalAvailableQuantity"))
                    {
                        summary["totalStockAvailable"] = invData.Sum(d => 
                            Convert.ToInt32(GetValue(d, "TotalAvailableQuantity") ?? 0));
                    }
                    if (firstItem != null && firstItem.ContainsKey("TotalAllocatedQuantity"))
                    {
                        summary["totalStockAllocated"] = invData.Sum(d => 
                            Convert.ToInt32(GetValue(d, "TotalAllocatedQuantity") ?? 0));
                    }
                }
            }

            return summary;
        }

        // ===== CONVERSION HELPERS =====

        private static List<Dictionary<string, object>>? ConvertToListDict(object? data)
        {
            if (data == null) return null;

            try
            {
                if (data is IEnumerable<dynamic> dynamicList)
                {
                    return dynamicList.Select(item => 
                    {
                        var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                        if (item is IDictionary<string, object> expando)
                        {
                            foreach (var kvp in expando)
                            {
                                dict[kvp.Key] = kvp.Value;
                            }
                        }
                        else
                        {
                            // Try to get properties via reflection
                            var props = item.GetType().GetProperties();
                            if (props != null && props.Length > 0)
                            {
                                foreach (var prop in props)
                                {
                                    dict[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                                }
                            }
                            else
                            {
                                // Fallback: serialize to JSON and parse into dictionary (handles DapperRow and other dynamics)
                                try
                                {
                                    var json = System.Text.Json.JsonSerializer.Serialize(item);
                                    using var doc = System.Text.Json.JsonDocument.Parse(json);
                                    var parsed = JsonElementToDictionary(doc.RootElement);
                                    foreach (var kv in parsed)
                                    {
                                        dict[kv.Key] = kv.Value ?? DBNull.Value;
                                    }
                                }
                                catch
                                {
                                    // give up for this item
                                }
                            }
                        }
                        return dict;
                    }).ToList();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        // Convert a JsonElement object to Dictionary<string, object> recursively
        private static Dictionary<string, object?> JsonElementToDictionary(System.Text.Json.JsonElement el)
        {
            var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            if (el.ValueKind != System.Text.Json.JsonValueKind.Object) return result;

            foreach (var prop in el.EnumerateObject())
            {
                result[prop.Name] = JsonElementToObject(prop.Value);
            }
            return result;
        }

        private static object? JsonElementToObject(System.Text.Json.JsonElement el)
        {
            switch (el.ValueKind)
            {
                case System.Text.Json.JsonValueKind.Object:
                    return JsonElementToDictionary(el);
                case System.Text.Json.JsonValueKind.Array:
                    var list = new List<object?>();
                    foreach (var item in el.EnumerateArray()) list.Add(JsonElementToObject(item));
                    return list;
                case System.Text.Json.JsonValueKind.String:
                    return el.GetString();
                case System.Text.Json.JsonValueKind.Number:
                    if (el.TryGetInt64(out var l)) return l;
                    if (el.TryGetDouble(out var d)) return d;
                    return el.GetRawText();
                case System.Text.Json.JsonValueKind.True:
                case System.Text.Json.JsonValueKind.False:
                    return el.GetBoolean();
                case System.Text.Json.JsonValueKind.Null:
                default:
                    return null;
            }
        }
    }
}
