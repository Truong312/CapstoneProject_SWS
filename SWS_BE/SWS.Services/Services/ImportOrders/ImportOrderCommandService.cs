using SWS.BusinessObjects.DTOs;
using SWS.Repositories.Repositories.ImportOrders;

namespace SWS.Services.ImportOrders;

public class ImportOrderCommandService : IImportOrderCommandService
{
    private readonly IImportOrderCommandRepository _repo;
    public ImportOrderCommandService(IImportOrderCommandRepository repo) => _repo = repo;

    public Task<CreateImportOrderResponse> CreateAsync(int createdBy, CreateImportOrderRequest req, CancellationToken ct = default)
        => _repo.CreateAsync(createdBy, req, ct);
}
