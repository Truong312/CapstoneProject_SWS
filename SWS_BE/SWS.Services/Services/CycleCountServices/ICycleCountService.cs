using SWS.Services.ApiModels.Commons;

namespace SWS.Services.Services.CycleCountServices
{
    public interface ICycleCountService
    {
        Task<ResultModel> StartCycleCountAsync();
        Task<ResultModel> UpdateCountedQuantityAsync(int cycleCountId,int productId, int countedQuantity);
        Task<ResultModel> UpdateCountedQuantityAsync(string cycleCountName, int productId, int countedQuantity);
        Task<ResultModel> FinalizeCycleCountAsync(int cycleCountId);
        Task<ResultModel> FinalizeCycleCountAsync(string cycleCountName);
    }
}
