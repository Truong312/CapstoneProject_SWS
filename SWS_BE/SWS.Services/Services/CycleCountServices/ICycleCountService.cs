using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Models;
using SWS.Services.ApiModels.Commons;

namespace SWS.Services.Services.CycleCountServices
{
    public interface ICycleCountService
    {
        Task<ResultModel> StartCycleCountAsync(int userId);
        Task<ResultModel> UpdateCountedQuantityAsync(int detailId, int countedQuantity);
        Task<ResultModel> FinalizeCycleCountAsync(int cycleCountId, int userId);
        Task<ResultModel> FinalizeCycleCountAsync(string cycleCountName, int userId);
    }
}
