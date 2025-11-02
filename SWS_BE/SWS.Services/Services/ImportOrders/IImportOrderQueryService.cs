using SWS.BusinessObjects.DTOs;

namespace SWS.Services.ImportOrders;

public interface IImportOrderQueryService
{
    Task<ImportOrderListResult> GetListAsync(ImportOrderListQuery query, CancellationToken ct = default);
    Task<ImportOrderDetailDto?> GetDetailAsync(int id, CancellationToken ct = default);
}
