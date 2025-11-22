using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Repositories.Repositories.InventoryRepo
{
    public interface IInventoryDashboardRepository
    {
        Task<decimal> GetTotalStockValueAsync();
        Task<int> GetLowStockCountAsync();
        Task<int> GetOutOfStockCountAsync();
        Task<decimal> GetInventoryTurnoverRateAsync();
    }

}
