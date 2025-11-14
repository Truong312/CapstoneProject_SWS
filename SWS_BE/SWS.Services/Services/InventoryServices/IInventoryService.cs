using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Models;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ApiModels.InventoryModel;

namespace SWS.Services.Services.InventoryServices
{
    public interface IInventoryService
    {
        Task<ResultModel<IEnumerable<InventoryResponse>>> GetAllInventoriesAsync();
        Task<ResultModel<InventoryResponse>> GetInventoryByIdAsync(int inventoryId);
        Task<ResultModel<InventoryResponse>> GetInventoryByProductIdAsync(int productId);
        Task<ResultModel> AddInventoryAsync(AddInventory addInventory);
        Task<ResultModel> UpdateInventoryAsync(int inventoryId, UpdateInventory updateInventory);
        Task<ResultModel> DeleteInventoryAsync(int inventoryId);
    }
}
