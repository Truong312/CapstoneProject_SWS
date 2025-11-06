using SWS.BusinessObjects.DTOs;

namespace SWS.Repositories.Repositories.ImportOrders;

public interface IImportOrderCommandRepository
{
    Task<CreateImportOrderResponse> CreateAsync(int createdBy, CreateImportOrderRequest req, CancellationToken ct = default);
}
