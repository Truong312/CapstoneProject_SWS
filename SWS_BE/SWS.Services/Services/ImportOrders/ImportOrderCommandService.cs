using SWS.BusinessObjects.DTOs;
using SWS.Repositories.UnitOfWork;

namespace SWS.Services.ImportOrders
{
    public class ImportOrderCommandService : IImportOrderCommandService
    {
        private readonly IUnitOfWork _uow;
        public ImportOrderCommandService(IUnitOfWork uow) => _uow = uow;

        public Task<CreateImportOrderResponse> CreateAsync(int createdBy, CreateImportOrderRequest req, CancellationToken ct = default)
            => _uow.ImportOrdersCommand.CreateAsync(createdBy, req, ct);
    }
}
