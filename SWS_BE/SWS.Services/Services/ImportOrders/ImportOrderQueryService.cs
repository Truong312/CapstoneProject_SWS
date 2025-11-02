using SWS.BusinessObjects.DTOs;
using SWS.Repositories.Repositories.ImportOrders;

namespace SWS.Services.ImportOrders;

public class ImportOrderQueryService : IImportOrderQueryService
{
    private readonly IImportOrderQueryRepository _repo;
    public ImportOrderQueryService(IImportOrderQueryRepository repo) => _repo = repo;

    public Task<ImportOrderListResult> GetListAsync(ImportOrderListQuery query, CancellationToken ct = default)
        => _repo.GetListAsync(query, ct);

    public Task<ImportOrderDetailDto?> GetDetailAsync(int id, CancellationToken ct = default)
        => _repo.GetDetailAsync(id, ct);
}
