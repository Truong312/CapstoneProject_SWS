using System.Threading.Tasks;
using SWS.BusinessObjects.Dtos;

namespace SWS.Services.Services.WarehouseAuthentication
{
    public interface IGoogleLoginService
    {
        GoogleAuthUrlDto GetGoogleLoginUrl();
        Task<GoogleUserInfoDto> GetUserInfoFromCodeAsync(string code);
    }
}
