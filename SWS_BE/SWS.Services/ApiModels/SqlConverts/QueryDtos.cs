namespace SWS.Services.ApiModels.SqlConverts;

public class SqlQueryRequest
{
    public string NaturalLanguage { get; set; } = string.Empty;
}

public class SqlQueryResultDto
{
    public string Sql { get; set; } = string.Empty;
    public object? Result { get; set; }
}

public class MultiSqlQueryResultDto
{
    public List<string> Queries { get; set; } = new();
    public List<QueryResultSet> Results { get; set; } = new();
    public int TotalQueries { get; set; }
}

public class QueryResultSet
{
    public string Query { get; set; } = string.Empty;
    public object? Data { get; set; }
    public int RowCount { get; set; }
    public string TableName { get; set; } = string.Empty;
}

/// <summary>
/// DTO cho kết quả tổng hợp thông minh - format đẹp cho FE
/// </summary>
public class SmartQueryResultDto
{
    /// <summary>
    /// Loại query: "product_search", "inventory_summary", "location_info", "statistics"
    /// </summary>
    public string QueryType { get; set; } = string.Empty;
    
    /// <summary>
    /// Tiêu đề mô tả kết quả
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Tổng số bản ghi
    /// </summary>
    public int TotalRecords { get; set; }
    
    /// <summary>
    /// Data chính đã được merge và format
    /// </summary>
    public List<Dictionary<string, object>> Data { get; set; } = new();
    
    /// <summary>
    /// Thống kê tổng hợp (nếu có)
    /// </summary>
    public Dictionary<string, object>? Summary { get; set; }
    
    /// <summary>
    /// Metadata bổ sung
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Response tổng hợp cho multiple queries
/// </summary>
public class ConsolidatedQueryResultDto
{
    /// <summary>
    /// Keyword tìm kiếm
    /// </summary>
    public string SearchKeyword { get; set; } = string.Empty;
    
    /// <summary>
    /// Tổng số queries đã execute
    /// </summary>
    public int TotalQueriesExecuted { get; set; }
    
    /// <summary>
    /// Danh sách kết quả đã được tổng hợp và format
    /// </summary>
    public List<SmartQueryResultDto> Results { get; set; } = new();
    
    /// <summary>
    /// Thống kê tổng quan
    /// </summary>
    public Dictionary<string, object>? OverallSummary { get; set; }
    
    /// <summary>
    /// Danh sách SQL queries gốc (để debug)
    /// </summary>
    public List<string>? OriginalQueries { get; set; }
}
