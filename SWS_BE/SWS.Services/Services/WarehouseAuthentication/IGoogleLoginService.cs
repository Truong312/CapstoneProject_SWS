using System.Threading.Tasks;
using SWS.Services.ApiModels;

namespace SWS.Services.Authentication
{
    public interface IGoogleLoginService
    {
        string GetGoogleLoginUrl();
        Task<GoogleUserInfo> GetUserInfoFromCodeAsync(string code);
    }
}
