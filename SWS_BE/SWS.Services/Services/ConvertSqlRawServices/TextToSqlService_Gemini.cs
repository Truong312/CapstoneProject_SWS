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
                    .Replace("{{SCHEMA}}", "(The schema has already been provided earlier. Use it as context.)")
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
                }
            };

            var json = JsonSerializer.Serialize(payload);
            Console.WriteLine($"[Gemini DEBUG] POST {url}");

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync(url, content, ct);

            var resultJson = await response.Content.ReadAsStringAsync(ct);
            Console.WriteLine($"[Gemini DEBUG] Raw Response: {resultJson}");

            if (!response.IsSuccessStatusCode)
            {
                throw new AppException($"Gemini call failed ({response.StatusCode}): {resultJson}");
            }

            // Parse Gemini response
            using var doc = JsonDocument.Parse(resultJson);

            if (!doc.RootElement.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
            {
                throw new AppException($"Gemini call failed ({response.StatusCode}): {resultJson}");
            }

            var sqlText = candidates[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString()?
                .Trim()
                .TrimEnd(';');

            // Clean SQL text
            sqlText = sqlText?
                .Replace("```sql", "", StringComparison.OrdinalIgnoreCase)
                .Replace("```", "", StringComparison.OrdinalIgnoreCase)
                .Trim();

            if (string.IsNullOrWhiteSpace(sqlText))
            {
                throw new AppException("Gemini returned empty SQL");
            }

            // Check if multiple queries (separated by |||)
            var queries = sqlText.Split("|||", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            if (queries.Length > 1)
            {
                // Multiple queries - execute all and return combined results
                var multiResult = new MultiSqlQueryResultDto
                {
                    Queries = queries.ToList(),
                    TotalQueries = queries.Length
                };

                foreach (var query in queries)
                {
                    var cleanQuery = CleanSqlQuery(query);
                    
                    if (string.IsNullOrWhiteSpace(cleanQuery) || !IsValidSelectQuery(cleanQuery))
                    {
                        Console.WriteLine($"[WARN] Skipping invalid query: {query}");
                        continue;
                    }

                    try
                    {
                        var rows = await _dapperRepo.QueryAsync<dynamic>(cleanQuery);
                        var rowsList = rows?.ToList() ?? new List<dynamic>();
                        
                        multiResult.Results.Add(new QueryResultSet
                        {
                            Query = cleanQuery,
                            Data = rowsList,
                            RowCount = rowsList.Count,
                            TableName = ExtractTableName(cleanQuery)
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to execute query: {cleanQuery}. Error: {ex.Message}");
                        multiResult.Results.Add(new QueryResultSet
                        {
                            Query = cleanQuery,
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
                        Sql = string.Join(" ||| ", queries),
                        Result = consolidated  // Trả về kết quả đã tổng hợp
                    },
                    StatusCode = StatusCodes.Status200OK
                };
            }
            else
            {
                // Single query - original behavior
                var sql = CleanSqlQuery(queries[0]);

                if (string.IsNullOrWhiteSpace(sql) || !IsValidSelectQuery(sql))
                {
                    throw new AppException($"Invalid SQL generated: {sql}");
                }

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

        private string CleanSqlQuery(string sql)
        {
            return sql
                .Replace("```sql", "", StringComparison.OrdinalIgnoreCase)
                .Replace("```", "", StringComparison.OrdinalIgnoreCase)
                .Replace("--", "")
                .Trim()
                .TrimEnd(';');
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
            var upper = sql.ToUpperInvariant();
            if (!upper.StartsWith("SELECT")) return false;

            var forbidden = new[] { "DELETE", "UPDATE", "INSERT", "DROP", "ALTER", "TRUNCATE", "EXEC", "MERGE" };
            return !forbidden.Any(word => upper.Contains(word));
        }
    }
}
