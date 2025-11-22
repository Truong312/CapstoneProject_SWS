using SWS.BusinessObjects.Dtos;
using SWS.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Repositories.Repositories.InventoryRepo
{
    public interface IInventoryRepository
    {
        Task<Inventory?> GetInventoryAsync(int productId, int locationId);
        Task<List<Inventory>> GetInventoryByProductAsync(int productId);

        Task<InventoryStatusSummaryDto> GetStockByStatusAsync();
        Task<List<ProductInventoryDto>> GetAllProductInventoryAsync();
    }

}
