namespace SWS.Services.ApiModels.Commons;

public class PagedRequestDto
{
    // 1-based
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Q { get; set; }
}

