using System;
using System.Collections.Generic;

namespace SWS.BusinessObjects.DTOs
{
    public record ReturnReasonDto(int ReasonId, string? ReasonCode, string? Description);

    public record ReturnStatusDto(string Status, int Count);

    public record ReturnOrderListItemDto(
        int ReturnOrderId,
        int? ExportOrderId,
        DateTime? CheckInTime,
        string? Status,
        string? Note,
        string? CheckedByName,
        string? ReviewedByName
    );

    public record ReturnOrderLineDto(
        int ReturnDetailId,
        int ProductId,
        string ProductName,
        int Quantity,
        int? ReasonId,
        string? ReasonCode,
        string? Note,
        int? ActionId,
        int? LocationId
    );

    public class ReturnOrderDetailDto
    {
        public ReturnOrderListItemDto Header { get; set; } = default!;
        public List<ReturnOrderLineDto> Lines { get; set; } = new();
    }
}
