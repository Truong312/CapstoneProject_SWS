using SWS.Services.ApiModels;
using SWS.Services.ApiModels.WarehouseUserModel;

namespace SWS.Services.Services.WarehouseAuthentication
{
    public interface IWarehouseAuthenticationService
    {
        Task<ResultModel> RegisterAsync(RegisterWarehouseRequest request);
        Task<ResultModel> LoginAsync(LoginWarehouseRequest request);
        Task<ResultModel> LoginWithGoogleAsync(string code);
        Task<ResultModel> GetUserByIdAsync(int userId);
        Task<ResultModel> UpdateUserAsync(int userId, RegisterWarehouseRequest request);
        Task<ResultModel> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    }
}
