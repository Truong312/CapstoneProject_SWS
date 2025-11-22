using SWS.BusinessObjects.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Services.Services.InventoryServices
{
    public interface IInventoryService
    {
        //Task<List<InventoryDto>> GetByProductAsync(int productId);
        Task<InventoryStatusSummaryDto> GetInventoryStatusSummaryAsync();

        Task<List<ProductInventoryDto>> GetAllProductInventoryAsync();
    }
}
