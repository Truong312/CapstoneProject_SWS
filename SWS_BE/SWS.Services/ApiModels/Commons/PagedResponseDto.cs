namespace SWS.Services.ApiModels.Commons;

public class PagedResponseDto<T>
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
}

