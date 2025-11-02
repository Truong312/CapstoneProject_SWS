using SWS.BusinessObjects.DTOs;
using SWS.Repositories.UnitOfWork;

namespace SWS.Services.ImportOrders
{
    public class ImportOrderQueryService : IImportOrderQueryService
    {
        private readonly IUnitOfWork _uow;
        public ImportOrderQueryService(IUnitOfWork uow) => _uow = uow;

        public Task<ImportOrderListResult> GetListAsync(ImportOrderListQuery query, CancellationToken ct = default)
            => _uow.ImportOrdersQuery.GetListAsync(query, ct);

        public Task<ImportOrderDetailDto?> GetDetailAsync(int id, CancellationToken ct = default)
            => _uow.ImportOrdersQuery.GetDetailAsync(id, ct);
    }
}
