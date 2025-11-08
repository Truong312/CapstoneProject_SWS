using System.Dynamic;
using SWS.Services.ApiModels.SqlConverts;

namespace SWS.Services.Helpers
{
    /// <summary>
    /// Helper để tổng hợp và format kết quả từ multiple queries thành dạng đẹp cho FE
    /// </summary>
    public class QueryResultConsolidator
    {
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
                        var productName = GetValue(invItem, "Name")?.ToString() ?? "Unknown";
                        var totalAvailable = Convert.ToInt32(GetValue(invItem, "TotalAvailableQuantity") ?? 0);
                        var totalAllocated = Convert.ToInt32(GetValue(invItem, "TotalAllocatedQuantity") ?? 0);

                        // Tìm location info tương ứng
                        var locationItems = locationData?.Where(l => 
                            GetValue(l, "Name")?.ToString() == productName).ToList() ?? new List<Dictionary<string, object>>();

                        var mergedItem = new Dictionary<string, object>
                        {
                            { "productName", productName },
                            { "totalAvailable", totalAvailable },
                            { "totalAllocated", totalAllocated },
                            { "totalInStock", totalAvailable + totalAllocated },
                            { "locations", locationItems.Select(loc => new Dictionary<string, object>
                            {
                                { "shelf", GetValue(loc, "ShelfID")?.ToString() ?? "" },
                                { "column", GetValue(loc, "ColumnNumber") ?? 0 },
                                { "row", GetValue(loc, "RowNumber") ?? 0 },
                                { "type", GetValue(loc, "LocationType")?.ToString() ?? "" },
                                { "quantity", GetValue(loc, "QuantityAvailable") ?? 0 }
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
                    var groupedByProduct = locationData.GroupBy(l => GetValue(l, "Name")?.ToString() ?? "Unknown");
                    
                    foreach (var group in groupedByProduct)
                    {
                        var locations = group.Select(loc => new Dictionary<string, object>
                        {
                            { "shelf", GetValue(loc, "ShelfID")?.ToString() ?? "" },
                            { "column", GetValue(loc, "ColumnNumber") ?? 0 },
                            { "row", GetValue(loc, "RowNumber") ?? 0 },
                            { "type", GetValue(loc, "LocationType")?.ToString() ?? "" },
                            { "quantity", GetValue(loc, "QuantityAvailable") ?? 0 }
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
                    var firstItem = invData.First();
                    if (firstItem.ContainsKey("TotalAvailableQuantity"))
                    {
                        summary["totalStockAvailable"] = invData.Sum(d => 
                            Convert.ToInt32(GetValue(d, "TotalAvailableQuantity") ?? 0));
                    }
                    if (firstItem.ContainsKey("TotalAllocatedQuantity"))
                    {
                        summary["totalStockAllocated"] = invData.Sum(d => 
                            Convert.ToInt32(GetValue(d, "TotalAllocatedQuantity") ?? 0));
                    }
                }
            }

            return summary;
        }

        private static List<Dictionary<string, object>>? ConvertToListDict(object? data)
        {
            if (data == null) return null;

            try
            {
                if (data is IEnumerable<dynamic> dynamicList)
                {
                    return dynamicList.Select(item => 
                    {
                        var dict = new Dictionary<string, object>();
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
                            foreach (var prop in props)
                            {
                                dict[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
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

        private static object? GetValue(Dictionary<string, object> dict, string key)
        {
            if (dict.TryGetValue(key, out var value))
                return value;
            
            // Try case-insensitive
            var kvp = dict.FirstOrDefault(k => k.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
            return kvp.Value;
        }

        private static List<string> GetColumnNames(Dictionary<string, object>? dict)
        {
            return dict?.Keys.ToList() ?? new List<string>();
        }
    }
}

