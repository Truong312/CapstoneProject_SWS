using SWS.BusinessObjects.DTOs;

namespace SWS.Services.ImportOrders;

public interface IImportOrderCommandService
{
    Task<CreateImportOrderResponse> CreateAsync(int createdBy, CreateImportOrderRequest req, CancellationToken ct = default);
}
