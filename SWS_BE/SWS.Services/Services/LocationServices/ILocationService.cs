using SWS.BusinessObjects.Models;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ApiModels.LocationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Services.Services.LocationServices
{
    public interface ILocationService
    {
        Task<ResultModel<IEnumerable<LocationResponse>>> GetAllLocationAsync();
        Task<ResultModel<IEnumerable<LocationResponse>>> GetAllLocationWithProductIdAsync(int productId);
        Task<ResultModel<LocationResponse>> GetLocationByIdAsync(int locationId);
        Task<ResultModel> CreateNewLocationAsync(CreateLocation createLocation);
        Task<ResultModel> UpdateLocationAsync(int locationId, UpdateLocation updateLocation);
        Task<ResultModel> DeleteLocationAsync(int locationId);
        Task<IEnumerable<Location>> GetLocations();
        Task<Location?> GetLocationDetail(int id);
        Task<IEnumerable<Inventory>> GetProductPlacement(int productId);
        Task<bool> AddInventory(Inventory inventory);
        Task<bool> UpdateInventory(int inventoryId, int newQuantity);
        Task<bool> RemoveInventory(int inventoryId);
        Task<Location?> SuggestLocation(int productId, int requiredQuantity);
    }
}
