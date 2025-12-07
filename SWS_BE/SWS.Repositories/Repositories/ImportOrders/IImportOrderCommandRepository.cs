using System.Threading;
using System.Threading.Tasks;
using SWS.BusinessObjects.DTOs;

namespace SWS.Repositories.Repositories.ImportOrders
{
    public interface IImportOrderCommandRepository
    {
        Task<CreateImportOrderResponse> CreateAsync(
            int createdBy,
            CreateImportOrderRequest req,
            CancellationToken ct = default);

        Task<bool> ReviewAsync(
            int importOrderId,
            int reviewerId,
            bool approve,
            string? note,
            CancellationToken ct = default);
    }
}
