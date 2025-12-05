using SWS.BusinessObjects.DTOs;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.ImportOrders;

public interface IImportOrderQueryRepository : IGenericRepository<ImportOrder>
{
    Task<ImportOrderListResult> GetListAsync(ImportOrderListQuery query, CancellationToken ct = default);
    Task<ImportOrderDetailDto?> GetDetailAsync(int id, CancellationToken ct = default);
}
