// File: SWS.Services/Services/ReturnOrders/ReturnOrderQueryService.cs
using SWS.BusinessObjects.DTOs;
using SWS.Repositories.UnitOfWork;   // 👈 dùng UoW, KHÔNG inject repo trực tiếp nữa

namespace SWS.Services.ReturnOrders
{
    public interface IReturnOrderQueryService
    {
        Task<List<ReturnOrderListItemDto>> GetListAsync(
            DateOnly? from, DateOnly? to, string? status,
            int? exportOrderId, int? checkedBy, int? reviewedBy);

        Task<ReturnOrderDetailDto?> GetDetailAsync(int id);
    }

    public class ReturnOrderQueryService : IReturnOrderQueryService
    {
        private readonly IUnitOfWork _uow;
        public ReturnOrderQueryService(IUnitOfWork uow) => _uow = uow;

        public async Task<List<ReturnOrderListItemDto>> GetListAsync(
            DateOnly? from, DateOnly? to, string? status,
            int? exportOrderId, int? checkedBy, int? reviewedBy)
        {
            var list = await _uow.ReturnOrdersQuery.GetListAsync(from, to, status, exportOrderId, checkedBy, reviewedBy);

            // Dùng named-args cho khớp DTO của bạn
            return list.Select(ro => new ReturnOrderListItemDto(
                ReturnOrderId: ro.ReturnOrderId,
                ExportOrderId: ro.ExportOrderId,
                CheckInTime: ro.CheckInTime,
                Status: ro.Status,
                Note: ro.Note,
                CheckedByName: ro.CheckedByNavigation?.FullName,
                ReviewedByName: ro.ReviewedByNavigation?.FullName
            )).ToList();
        }

        public async Task<ReturnOrderDetailDto?> GetDetailAsync(int id)
        {
            var header = await _uow.ReturnOrdersQuery.GetDetailAsync(id);
            if (header == null) return null;

            var dto = new ReturnOrderDetailDto
            {
                Header = new ReturnOrderListItemDto(
                    ReturnOrderId: header.ReturnOrderId,
                    ExportOrderId: header.ExportOrderId,
                    CheckInTime: header.CheckInTime,
                    Status: header.Status,
                    Note: header.Note,
                    CheckedByName: header.CheckedByNavigation?.FullName,
                    ReviewedByName: header.ReviewedByNavigation?.FullName
                ),
                Lines = header.ReturnOrderDetails.Select(d => new ReturnOrderLineDto(
                    ReturnDetailId: d.ReturnDetailId,
                    ProductId: d.ProductId,
                    ProductName: d.Product?.Name,              // 👈 null-safe
                    Quantity: d.Quantity,
                    ReasonId: d.ReasonId,
                    ReasonCode: d.Reason?.ReasonCode,         // 👈 null-safe
                    Note: d.Note,
                    ActionId: d.ActionId,
                    LocationId: d.LocationId
                )).ToList()
            };
            return dto;
        }
    }
}
