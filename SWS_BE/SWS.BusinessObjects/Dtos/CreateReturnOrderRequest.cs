using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SWS.BusinessObjects.DTOs;

public class CreateReturnOrderItemRequest
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be > 0")]
    public int Quantity { get; set; }

    public int? ReasonId { get; set; }

    public int? ActionId { get; set; }

    public int? LocationId { get; set; }

    public string? Note { get; set; }
}

public class CreateReturnOrderRequest
{

    public int? ExportOrderId { get; set; }

    public string? Note { get; set; }

    [MinLength(1, ErrorMessage = "Return order must have at least one item.")]
    public List<CreateReturnOrderItemRequest> Items { get; set; } = new();
}

public sealed record CreateReturnOrderResponse(int ReturnOrderId);
