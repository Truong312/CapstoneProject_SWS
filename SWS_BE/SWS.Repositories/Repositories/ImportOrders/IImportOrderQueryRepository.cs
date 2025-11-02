using SWS.BusinessObjects.DTOs;

namespace SWS.Repositories.Repositories.ImportOrders;

public interface IImportOrderQueryRepository
{
    Task<ImportOrderListResult> GetListAsync(ImportOrderListQuery query, CancellationToken ct = default);
    Task<ImportOrderDetailDto?> GetDetailAsync(int id, CancellationToken ct = default);
}
