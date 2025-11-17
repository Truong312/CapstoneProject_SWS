using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SWS.BusinessObjects.AppSettings;
using SWS.Services.ApiModels.SqlConverts;
using SWS.BusinessObjects.Exceptions;
using SWS.Repositories.Generic;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ConvertSqlRawServices;
using System.Text.RegularExpressions;

namespace SWS.Services.Services.ConvertSqlRawServices
{
    public class TextToSqlService_Gemini : ITextToSqlService
    {
        private readonly HttpClient _httpClient;
        private readonly IDapperRepository _dapperRepo;
        private readonly GeminiSettings _settings;
        private readonly string _promptTemplate;
        private readonly string _schemaJson;

        private const int MAX_RETRIES = 2;

        private readonly string[] FORBIDDEN_KEYWORDS =
            { "DELETE", "UPDATE", "INSERT", "DROP", "ALTER", "TRUNCATE", "EXEC", "EXECUTE", "MERGE", "CREATE" };

        public TextToSqlService_Gemini(
            IOptions<GeminiSettings> settings,
            IDapperRepository dapperRepo,
            IHttpClientFactory httpClientFactory)
        {
            _settings = settings.Value;
            _dapperRepo = dapperRepo;
            _httpClient = httpClientFactory.CreateClient();

            // Load base prompt template
            var promptPath = Path.Combine(AppContext.BaseDirectory, "Prompts", "base_prompt.txt");
            _promptTemplate = File.ReadAllText(promptPath);

            // Load schema JSON
            var schemaPath = Path.Combine(AppContext.BaseDirectory, "Prompts", "schema.json");
            _schemaJson = File.ReadAllText(schemaPath);
        }

        public async Task<ResultModel<SqlQueryResultDto>> QueryAsync(string naturalLanguage,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(naturalLanguage))
                throw new AppException("invalid_input", "Natural language query is required",
                    StatusCodes.Status400BadRequest);

            int retryCount = 0;
            GeminiSqlResponse? geminiResponse = null;

            // Retry logic để đảm bảo có kết quả hợp lệ
            while (retryCount <= MAX_RETRIES)
            {
                try
                {
                    var responseText = await CallGeminiAPI(naturalLanguage, ct);

                    // Parse JSON response từ Gemini
                    geminiResponse = ParseGeminiResponse(responseText);

                    if (geminiResponse != null && geminiResponse.Status == "ok" && 
                        !string.IsNullOrWhiteSpace(geminiResponse.MainQuery))
                    {
                        break; // Success
                    }

                    if (geminiResponse != null && geminiResponse.Status == "error")
                    {
                        // Model trả về lỗi rõ ràng (ví dụ: thiếu bảng, cột không tồn tại)
                        return new ResultModel<SqlQueryResultDto>
                        {
                            IsSuccess = false,
                            ResponseCode = "validation_error",
                            Message = geminiResponse.ErrorMessage ?? "Schema validation failed",
                            Data = null,
                            StatusCode = StatusCodes.Status400BadRequest
                        };
                    }

                    Console.WriteLine($"[WARN] Invalid Gemini response, retry {retryCount + 1}/{MAX_RETRIES}");
                    retryCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Gemini call failed: {ex.Message}");
                    if (retryCount >= MAX_RETRIES)
                    {
                        if (ex is AppException aex)
                        {
                            return new ResultModel<SqlQueryResultDto>
                            {
                                IsSuccess = false,
                                ResponseCode = aex.Code ?? "error",
                                Message = aex.Message,
                                Data = null,
                                StatusCode = aex.StatusCode
                            };
                        }

                        return new ResultModel<SqlQueryResultDto>
                        {
                            IsSuccess = false,
                            ResponseCode = "generation_failed",
                            Message = ex.Message,
                            Data = null,
                            StatusCode = StatusCodes.Status502BadGateway
                        };
                    }

                    retryCount++;
                }
            }

            if (geminiResponse == null || string.IsNullOrWhiteSpace(geminiResponse.MainQuery))
            {
                return new ResultModel<SqlQueryResultDto>
                {
                    IsSuccess = false,
                    ResponseCode = "generation_failed",
                    Message = "Failed to generate valid SQL after retries",
                    Data = null,
                    StatusCode = StatusCodes.Status502BadGateway
                };
            }

            // Clean và validate main_query
            var mainQuery = CleanSqlQuery(geminiResponse.MainQuery);

            Console.WriteLine($"[DEBUG] Cleaned main_query (first 300 chars): {(mainQuery.Length > 300 ? mainQuery.Substring(0, 300) + "..." : mainQuery)}");
            Console.WriteLine($"[DEBUG] Cleaned main_query starts with: {mainQuery.Substring(0, Math.Min(50, mainQuery.Length))}");

            if (!IsValidSelectQuery(mainQuery))
            {
                Console.WriteLine($"[ERROR] SQL validation failed. Full cleaned SQL: {mainQuery}");
                return new ResultModel<SqlQueryResultDto>
                {
                    IsSuccess = false,
                    ResponseCode = "invalid_sql",
                    Message = "Generated main_query is not a valid SELECT statement",
                    Data = null,
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };
            }

            // Execute main_query và trả về kết quả (chỉ 1 bảng chính)
            try
            {
                var rows = await _dapperRepo.QueryAsync<dynamic>(mainQuery);
                var rowsList = rows?.ToList() ?? new List<dynamic>();

                return new ResultModel<SqlQueryResultDto>
                {
                    IsSuccess = true,
                    ResponseCode = "success",
                    Message = $"Tìm thấy {rowsList.Count} kết quả",
                    Data = new SqlQueryResultDto 
                    { 
                        Sql = mainQuery, 
                        Result = rowsList,
                        ColumnMapping = geminiResponse.ColumnMapping,
                        // Optionally include metadata from Gemini response
                        Metadata = new Dictionary<string, object>
                        {
                            { "main_table_schema", geminiResponse.MainTableSchema ?? new List<ColumnSchema>() },
                            { "validation", geminiResponse.Validation ?? new ValidationInfo() },
                            { "summary_query_available", !string.IsNullOrWhiteSpace(geminiResponse.SummaryQuery) }
                        }
                    },
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to execute main_query: {mainQuery}. Error: {ex.Message}");
                return new ResultModel<SqlQueryResultDto>
                {
                    IsSuccess = false,
                    ResponseCode = "execution_failed",
                    Message = $"Failed to execute query: {ex.Message}",
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        private async Task<string> CallGeminiAPI(string naturalLanguage, CancellationToken ct)
        {
            // Build the final prompt
            var prompt = _promptTemplate
                .Replace("{{SCHEMA}}", _schemaJson)
                .Replace("{{QUESTION}}", naturalLanguage);

            var url =
                $"https://generativelanguage.googleapis.com/v1/models/{_settings.Model}:generateContent?key={_settings.ApiKey}";

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.1,
                    maxOutputTokens = 4096,
                    topP = 0.8,
                    topK = 10
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(url, content, ct);
            var resultJson = await response.Content.ReadAsStringAsync(ct);

            Console.WriteLine($"[DEBUG] Gemini raw response (truncated): {(resultJson.Length > 2000 ? resultJson.Substring(0, 2000) + "..." : resultJson)}");

            if (!response.IsSuccessStatusCode)
            {
                throw new AppException("gemini_api_error", $"Gemini API failed ({response.StatusCode}): {resultJson}",
                    StatusCodes.Status502BadGateway);
            }

            using var doc = JsonDocument.Parse(resultJson);
            JsonElement root = doc.RootElement;

            string? extracted = null;

            if (root.TryGetProperty("candidates", out var candidates) && candidates.ValueKind == JsonValueKind.Array &&
                candidates.GetArrayLength() > 0)
            {
                var first = candidates[0];
                extracted = ExtractTextFromElement(first);
            }
            else
            {
                extracted = ExtractTextFromElement(root);
            }

            if (string.IsNullOrWhiteSpace(extracted))
            {
                var snippet = resultJson.Length > 1000 ? resultJson.Substring(0, 1000) + "..." : resultJson;
                throw new AppException("gemini_no_text",
                    $"Gemini returned no usable text in response. Raw response (truncated): {snippet}",
                    StatusCodes.Status502BadGateway);
            }

            return extracted.Trim();
        }

        private GeminiSqlResponse? ParseGeminiResponse(string responseText)
        {
            try
            {
                // Gemini có thể trả về JSON trong fenced code block hoặc trực tiếp
                var jsonText = responseText.Trim();

                // Remove markdown fenced code block nếu có
                // Pattern: ```json\n{...}\n``` hoặc ```\n{...}\n```
                if (jsonText.StartsWith("```"))
                {
                    // Tìm vị trí của dòng đầu tiên sau ```
                    var firstNewline = jsonText.IndexOf('\n');
                    if (firstNewline > 0)
                    {
                        jsonText = jsonText.Substring(firstNewline + 1).Trim();
                    }
                }

                // Remove trailing ``` nếu có
                if (jsonText.EndsWith("```"))
                {
                    var lastBacktick = jsonText.LastIndexOf("```");
                    jsonText = jsonText.Substring(0, lastBacktick).Trim();
                }

                // Đảm bảo JSON bắt đầu bằng {
                var jsonStart = jsonText.IndexOf('{');
                if (jsonStart > 0)
                {
                    jsonText = jsonText.Substring(jsonStart);
                }

                // Đảm bảo JSON kết thúc bằng }
                var jsonEnd = jsonText.LastIndexOf('}');
                if (jsonEnd > 0 && jsonEnd < jsonText.Length - 1)
                {
                    jsonText = jsonText.Substring(0, jsonEnd + 1);
                }

                Console.WriteLine($"[DEBUG] Cleaned JSON text (first 500 chars): {(jsonText.Length > 500 ? jsonText.Substring(0, 500) + "..." : jsonText)}");

                // Parse JSON
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true
                };

                var response = JsonSerializer.Deserialize<GeminiSqlResponse>(jsonText, options);

                if (response != null)
                {
                    var mainQueryLength = response.MainQuery?.Length ?? 0;
                    var previewLength = Math.Min(100, mainQueryLength);
                    var mainQueryPreview = mainQueryLength > 0 ? response.MainQuery!.Substring(0, previewLength) : "(empty)";
                    Console.WriteLine($"[DEBUG] Parsed Gemini response: status={response.Status}, main_query length={mainQueryLength}, main_query preview={mainQueryPreview}");
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to parse Gemini response as JSON: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                // Try to extract JSON từ text nếu có văn bản bao quanh
                return TryExtractJsonFromText(responseText);
            }
        }

        private GeminiSqlResponse? TryExtractJsonFromText(string text)
        {
            try
            {
                // Tìm JSON object trong text (bắt đầu với { và kết thúc với })
                var startIndex = text.IndexOf('{');
                var endIndex = text.LastIndexOf('}');

                if (startIndex >= 0 && endIndex > startIndex)
                {
                    var jsonText = text.Substring(startIndex, endIndex - startIndex + 1);
                    
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        AllowTrailingCommas = true
                    };

                    return JsonSerializer.Deserialize<GeminiSqlResponse>(jsonText, options);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to extract JSON from text: {ex.Message}");
            }

            return null;
        }

        // Helper: recursively search a JsonElement for the first sensible string output
        private static string? ExtractTextFromElement(JsonElement element)
        {
            try
            {
                if (element.ValueKind == JsonValueKind.Object)
                {
                    // Direct text property
                    if (element.TryGetProperty("text", out var textProp) && textProp.ValueKind == JsonValueKind.String)
                        return textProp.GetString();

                    // content => parts => [ { text: "..." } ] pattern
                    if (element.TryGetProperty("content", out var contentProp))
                    {
                        // If content is string
                        if (contentProp.ValueKind == JsonValueKind.String)
                            return contentProp.GetString();

                        // If content contains parts array
                        if (contentProp.ValueKind == JsonValueKind.Object &&
                            contentProp.TryGetProperty("parts", out var partsProp) &&
                            partsProp.ValueKind == JsonValueKind.Array && partsProp.GetArrayLength() > 0)
                        {
                            var firstPart = partsProp[0];
                            if (firstPart.ValueKind == JsonValueKind.Object &&
                                firstPart.TryGetProperty("text", out var partText) &&
                                partText.ValueKind == JsonValueKind.String)
                                return partText.GetString();

                            if (firstPart.ValueKind == JsonValueKind.String)
                                return firstPart.GetString();
                        }

                        // Otherwise recurse into content
                        var recursive = ExtractTextFromElement(contentProp);
                        if (!string.IsNullOrWhiteSpace(recursive)) return recursive;
                    }

                    // Some responses use an "output" or "candidates" sub-objects
                    if (element.TryGetProperty("output", out var outputProp))
                    {
                        var rec = ExtractTextFromElement(outputProp);
                        if (!string.IsNullOrWhiteSpace(rec)) return rec;
                    }

                    // Generic recursion through object properties
                    foreach (var prop in element.EnumerateObject())
                    {
                        var rec = ExtractTextFromElement(prop.Value);
                        if (!string.IsNullOrWhiteSpace(rec)) return rec;
                    }
                }

                if (element.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in element.EnumerateArray())
                    {
                        var rec = ExtractTextFromElement(item);
                        if (!string.IsNullOrWhiteSpace(rec)) return rec;
                    }
                }

                if (element.ValueKind == JsonValueKind.String)
                {
                    var s = element.GetString();
                    if (!string.IsNullOrWhiteSpace(s)) return s;
                }
            }
            catch
            {
                // Swallow parse exceptions here and let caller handle absence of text
            }

            return null;
        }

        private string CleanSqlQuery(string sql)
        {
            if (sql == null) return string.Empty;

            // Remove invisible unicode characters (BOM, zero-width spaces) that can break StartsWith checks
            sql = sql.Replace("\uFEFF", "").Replace("\u200B", "");

            var cleaned = sql
                .Replace("```sql", "", StringComparison.OrdinalIgnoreCase)
                .Replace("```", "", StringComparison.OrdinalIgnoreCase)
                .Trim()
                .TrimEnd(';');

            // Remove inline comments (-- ...)
            cleaned = Regex.Replace(cleaned, @"--[^\r\n]*", "", RegexOptions.Multiline);

            // Fix common table name errors
            cleaned = Regex.Replace(cleaned, @"\bImportOrders\b", "ImportOrder", RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"\bExportOrders\b", "ExportOrder", RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"\bProducts\b", "Product", RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"\bUsers\b", "User", RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"\bInventories\b", "Inventory", RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"\bBusinessPartners\b", "BusinessPartner", RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"\bReturnOrders\b", "ReturnOrder", RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"\bLocations\b", "Location", RegexOptions.IgnoreCase);
            // Additional mappings: detail tables often have different names in model outputs
            cleaned = Regex.Replace(cleaned, @"\bImportOrderDetail\b", "ImportDetail", RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"\bExportOrderDetail\b", "ExportDetail", RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"\bImportOrderDetails\b", "ImportDetail", RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"\bExportOrderDetails\b", "ExportDetail", RegexOptions.IgnoreCase);

            return cleaned.Trim();
        }

        private bool IsValidSelectQuery(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return false;

            var upper = sql.ToUpperInvariant().Trim();
            
            // Accept queries starting with WITH (CTE) or SELECT
            bool startsWithValidKeyword = upper.StartsWith("WITH") || upper.StartsWith("SELECT");
            
            if (!startsWithValidKeyword)
            {
                Console.WriteLine($"[DEBUG] Validation failed: does not start with WITH or SELECT. Starts with: {upper.Substring(0, Math.Min(100, upper.Length))}");
                return false;
            }
            
            // Must contain SELECT somewhere (for WITH...SELECT pattern)
            if (upper.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase) < 0)
            {
                Console.WriteLine($"[DEBUG] Validation failed: does not contain SELECT keyword");
                return false;
            }

            // No forbidden keywords anywhere
            foreach (var keyword in FORBIDDEN_KEYWORDS)
            {
                if (upper.Contains(keyword))
                {
                    Console.WriteLine($"[DEBUG] Validation failed: contains forbidden keyword '{keyword}'");
                    return false;
                }
            }

            return true;
        }
    }

    // ===== DTO classes cho Gemini JSON response =====
    
    public class GeminiSqlResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public string Status { get; set; } = "ok";
        
        [System.Text.Json.Serialization.JsonPropertyName("main_query")]
        public string MainQuery { get; set; } = string.Empty;
        
        [System.Text.Json.Serialization.JsonPropertyName("summary_query")]
        public string SummaryQuery { get; set; } = string.Empty;
        
        [System.Text.Json.Serialization.JsonPropertyName("main_table_schema")]
        public List<ColumnSchema>? MainTableSchema { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("column_mapping")]
        public Dictionary<string, string>? ColumnMapping { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("validation")]
        public ValidationInfo? Validation { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("error_message")]
        public string? ErrorMessage { get; set; }
    }

    public class ColumnSchema
    {
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
        [System.Text.Json.Serialization.JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        
        [System.Text.Json.Serialization.JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;
        
        [System.Text.Json.Serialization.JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }

    public class ValidationInfo
    {
        [System.Text.Json.Serialization.JsonPropertyName("used_tables")]
        public List<string> UsedTables { get; set; } = new();
        
        [System.Text.Json.Serialization.JsonPropertyName("used_columns")]
        public List<string> UsedColumns { get; set; } = new();
        
        [System.Text.Json.Serialization.JsonPropertyName("joins")]
        public List<string> Joins { get; set; } = new();
        
        [System.Text.Json.Serialization.JsonPropertyName("assumptions")]
        public List<string> Assumptions { get; set; } = new();
    }
}

