using SWS.BusinessObjects.DTOs;
using SWS.Repositories.Repositories.ReturnRepo;

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
        private readonly IReturnOrderQueryRepository _repo;
        public ReturnOrderQueryService(IReturnOrderQueryRepository repo) => _repo = repo;

        public async Task<List<ReturnOrderListItemDto>> GetListAsync(
            DateOnly? from, DateOnly? to, string? status,
            int? exportOrderId, int? checkedBy, int? reviewedBy)
        {
            var list = await _repo.GetListAsync(from, to, status, exportOrderId, checkedBy, reviewedBy);
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
            var header = await _repo.GetDetailAsync(id);
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
                    ProductName: d.Product.Name,
                    Quantity: d.Quantity,
                    ReasonId: d.ReasonId,
                    ReasonCode: d.Reason?.ReasonCode,
                    Note: d.Note,
                    ActionId: d.ActionId,
                    LocationId: d.LocationId
                )).ToList()
            };
            return dto;
        }
    }
}
