using SWS.BusinessObjects.DTOs;
using SWS.Repositories.UnitOfWork;

namespace SWS.Services.ReturnOrders
{
    public interface IReturnOrderCommandService
    {
        Task<CreateReturnOrderResponse> CreateAsync(
            int createdBy,
            CreateReturnOrderRequest req,
            CancellationToken ct = default);
    }

    public class ReturnOrderCommandService : IReturnOrderCommandService
    {
        private readonly IUnitOfWork _uow;
        public ReturnOrderCommandService(IUnitOfWork uow) => _uow = uow;

        public Task<CreateReturnOrderResponse> CreateAsync(
            int createdBy,
            CreateReturnOrderRequest req,
            CancellationToken ct = default)
            => _uow.ReturnOrdersCommand.CreateAsync(createdBy, req, ct);
    }
}
