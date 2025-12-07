using SWS.BusinessObjects.Dtos;
using SWS.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;

namespace SWS.Repositories.Repositories.InventoryRepo
{
    public interface IInventoryRepository: IGenericRepository<Inventory>
    {
        Task<Inventory> GetByProductId(int productId);
        Task UpdateQuantity(int productId, int quantity);
        Task<Inventory?> GetInventoryAsync(int productId, int locationId);
        Task<List<Inventory>> GetInventoryByProductAsync(int productId);

        Task<InventoryStatusSummaryDto> GetStockByStatusAsync();
        Task<List<ProductInventoryDto>> GetAllProductInventoryAsync();
    }

}
