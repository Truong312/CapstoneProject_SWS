using System.ComponentModel.DataAnnotations;

namespace SWS.BusinessObjects.DTOs;

public sealed record ImportOrderListQuery(
    string? Q = null,
    int? ProviderId = null,
    string? Status = null,
    DateOnly? From = null,
    DateOnly? To = null,
    int Page = 1,
    int PageSize = 20
);

public sealed record ImportOrderListItemDto(
    int ImportOrderId,
    string InvoiceNumber,
    DateOnly OrderDate,
    string ProviderName,
    string? Status,
    int TotalItems,
    string? CreatedByName
);

public sealed record ImportOrderListResult(
    int Total,
    int Page,
    int PageSize,
    IEnumerable<ImportOrderListItemDto> Items
);

public sealed record ImportOrderDetailItemDto(
    int ImportDetailId,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal? ImportPrice
);

public sealed record ImportOrderDetailDto(
    int ImportOrderId,
    string InvoiceNumber,
    DateOnly OrderDate,
    int ProviderId,
    string ProviderName,
    string? Status,
    DateOnly? CreatedDate,
    int? CreatedBy,
    string? CreatedByName,
    IEnumerable<ImportOrderDetailItemDto> Items
);

public sealed class CreateImportOrderRequest
{
    public int ProviderId { get; set; }
    public DateOnly? OrderDate { get; set; }
    public string? InvoiceNumber { get; set; } // nếu null sẽ tự sinh

    [Required, MinLength(1, ErrorMessage = "Danh sách sản phẩm phải có ít nhất 1 dòng.")]
    public List<CreateImportOrderItem> Items { get; set; } = new();
}

public sealed class CreateImportOrderItem
{
    [Required] public int ProductId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải > 0")]
    public int Quantity { get; set; }
    [Range(0, double.MaxValue, ErrorMessage = "Giá nhập không âm")]
    public decimal? ImportPrice { get; set; }
}

public sealed record CreateImportOrderResponse(
    int ImportOrderId,
    string InvoiceNumber
);
