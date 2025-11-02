using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SWS.BusinessObjects.AppSettings;
using SWS.Services.ApiModels.SqlConverts;
using SWS.BusinessObjects.Exceptions;
using SWS.Repositories.Generic;
using SWS.Services.ApiModels;
using SWS.Services.ConvertSqlRawServices;
using SWS.Services.Helpers;
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

        // Flag to mark if schema has already been sent to the model
        private bool _schemaSent;

        private const int MAX_RETRIES = 2;
        private readonly string[] FORBIDDEN_KEYWORDS = { "DELETE", "UPDATE", "INSERT", "DROP", "ALTER", "TRUNCATE", "EXEC", "EXECUTE", "MERGE", "CREATE" };

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

            // Load schema JSON (big schema, we only want to send it once)
            var schemaPath = Path.Combine(AppContext.BaseDirectory, "Prompts", "schema.json");
            _schemaJson = File.ReadAllText(schemaPath);

            // default _schemaSent to false (implicit)
            _schemaSent = false;
        }

        public async Task<ResultModel<SqlQueryResultDto>> QueryAsync(string naturalLanguage, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(naturalLanguage))
                throw new AppException("Natural language query is required");

            int retryCount = 0;
            string? sqlText = null;

            // Retry logic để đảm bảo có kết quả hợp lệ
            while (retryCount <= MAX_RETRIES)
            {
                try
                {
                    sqlText = await CallGeminiAPI(naturalLanguage, ct);
                    
                    // Validate format
                    if (ValidateSqlFormat(sqlText))
                        break;
                    
                    Console.WriteLine($"[WARN] Invalid SQL format, retry {retryCount + 1}/{MAX_RETRIES}");
                    retryCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Gemini call failed: {ex.Message}");
                    if (retryCount >= MAX_RETRIES) throw;
                    retryCount++;
                }
            }

            if (string.IsNullOrWhiteSpace(sqlText))
            {
                throw new AppException("Failed to generate valid SQL after retries");
            }

            // Check if multiple queries (separated by |||)
            var queries = sqlText.Split("|||", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            // Clean và validate từng query
            var validQueries = new List<string>();
            foreach (var query in queries)
            {
                var cleanQuery = CleanSqlQuery(query);
                if (!string.IsNullOrWhiteSpace(cleanQuery) && IsValidSelectQuery(cleanQuery))
                {
                    validQueries.Add(cleanQuery);
                }
                else
                {
                    Console.WriteLine($"[WARN] Skipping invalid query: {query}");
                }
            }

            if (validQueries.Count == 0)
            {
                throw new AppException("No valid queries generated");
            }

            if (validQueries.Count > 1)
            {
                // Multiple queries - execute all and return combined results
                var multiResult = new MultiSqlQueryResultDto
                {
                    Queries = validQueries,
                    TotalQueries = validQueries.Count
                };

                foreach (var query in validQueries)
                {
                    try
                    {
                        var rows = await _dapperRepo.QueryAsync<dynamic>(query);
                        var rowsList = rows?.ToList() ?? new List<dynamic>();
                        
                        multiResult.Results.Add(new QueryResultSet
                        {
                            Query = query,
                            Data = rowsList,
                            RowCount = rowsList.Count,
                            TableName = ExtractTableName(query)
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to execute query: {query}. Error: {ex.Message}");
                        multiResult.Results.Add(new QueryResultSet
                        {
                            Query = query,
                            Data = new { error = ex.Message },
                            RowCount = 0,
                            TableName = "Error"
                        });
                    }
                }

                // ✨ TỔNG HỢP KẾT QUẢ THÔNG MINH
                var consolidated = QueryResultConsolidator.Consolidate(multiResult, naturalLanguage);

                // Return consolidated result
                return new ResultModel<SqlQueryResultDto>
                {
                    IsSuccess = true,
                    ResponseCode = "success",
                    Message = $"Tìm thấy {consolidated.Results.Sum(r => r.TotalRecords)} kết quả",
                    Data = new SqlQueryResultDto 
                    { 
                        Sql = string.Join(" ||| ", validQueries),
                        Result = consolidated
                    },
                    StatusCode = StatusCodes.Status200OK
                };
            }
            else
            {
                // Single query
                var sql = validQueries[0];
                var rows = await _dapperRepo.QueryAsync<dynamic>(sql);

                return new ResultModel<SqlQueryResultDto>
                {
                    IsSuccess = true,
                    ResponseCode = "success",
                    Message = "200",
                    Data = new SqlQueryResultDto { Sql = sql, Result = rows },
                    StatusCode = StatusCodes.Status200OK
                };
            }
        }

        private async Task<string> CallGeminiAPI(string naturalLanguage, CancellationToken ct)
        {
            // Build the final prompt
            string prompt;
            if (!_schemaSent)
            {
                prompt = _promptTemplate
                    .Replace("{{SCHEMA}}", _schemaJson)
                    .Replace("{{QUESTION}}", naturalLanguage);
                _schemaSent = true;
            }
            else
            {
                prompt = _promptTemplate
                    .Replace("{{SCHEMA}}", "(Schema đã gửi trước đó)")
                    .Replace("{{QUESTION}}", naturalLanguage);
            }

            var url = $"https://generativelanguage.googleapis.com/v1/models/{_settings.Model}:generateContent?key={_settings.ApiKey}";

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
                    temperature = 0.1,  // Giảm nhiệt độ để output ổn định hơn
                    maxOutputTokens = 2048,
                    topP = 0.8,
                    topK = 10
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            using var response = await _httpClient.PostAsync(url, content, ct);
            var resultJson = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                throw new AppException($"Gemini API failed ({response.StatusCode}): {resultJson}");
            }

            using var doc = JsonDocument.Parse(resultJson);

            if (!doc.RootElement.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
            {
                throw new AppException("Gemini returned no candidates");
            }

            var sqlText = candidates[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString()?
                .Trim();

            return sqlText ?? string.Empty;
        }

        private bool ValidateSqlFormat(string? sqlText)
        {
            if (string.IsNullOrWhiteSpace(sqlText))
                return false;

            // Clean first
            var cleaned = sqlText
                .Replace("```sql", "", StringComparison.OrdinalIgnoreCase)
                .Replace("```", "", StringComparison.OrdinalIgnoreCase)
                .Trim();

            // Check nếu có SELECT
            if (!cleaned.Contains("SELECT", StringComparison.OrdinalIgnoreCase))
                return false;

            // Check không có từ khóa nguy hiểm
            var upper = cleaned.ToUpperInvariant();
            if (FORBIDDEN_KEYWORDS.Any(keyword => upper.Contains(keyword)))
                return false;

            // Check format của multiple queries
            if (cleaned.Contains("|||"))
            {
                var queries = cleaned.Split("|||", StringSplitOptions.RemoveEmptyEntries);
                // Mỗi query phải bắt đầu bằng SELECT
                return queries.All(q => q.Trim().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase));
            }

            return true;
        }

        private string CleanSqlQuery(string sql)
        {
            // Remove markdown, comments, semicolons
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

            return cleaned.Trim();
        }

        private string ExtractTableName(string sql)
        {
            try
            {
                // Simple extraction: find "FROM tablename" pattern
                var fromIndex = sql.IndexOf("FROM", StringComparison.OrdinalIgnoreCase);
                if (fromIndex == -1) return "Unknown";

                var afterFrom = sql.Substring(fromIndex + 4).Trim();
                var tableName = afterFrom.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries)[0];
                
                return tableName.Replace("[", "").Replace("]", "");
            }
            catch
            {
                return "Unknown";
            }
        }

        private bool IsValidSelectQuery(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return false;

            var upper = sql.ToUpperInvariant();
            
            // Must start with SELECT
            if (!upper.TrimStart().StartsWith("SELECT"))
                return false;

            // No forbidden keywords
            return !FORBIDDEN_KEYWORDS.Any(word => upper.Contains(word));
        }
    }
}
