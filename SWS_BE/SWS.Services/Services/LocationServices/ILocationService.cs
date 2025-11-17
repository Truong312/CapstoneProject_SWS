using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ApiModels.LocationModel;

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
    }
}
