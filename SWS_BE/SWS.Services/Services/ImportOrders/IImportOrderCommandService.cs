using SWS.BusinessObjects.DTOs;
using SWS.Services.ApiModels;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ApiModels.ImportOrders;
using System.Threading;
using System.Threading.Tasks;

namespace SWS.Services.ImportOrders
{
    public interface IImportOrderCommandService
    {
        Task<CreateImportOrderResponse> CreateAsync(
            int createdBy,
            CreateImportOrderRequest req,
            CancellationToken ct = default);

        Task<ResultModel<bool>> ReviewAsync(
            int importOrderId,
            int reviewerId,
            ReviewImportOrderRequest req,
            CancellationToken ct = default);
    }
}
