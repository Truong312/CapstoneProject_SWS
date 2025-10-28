using Microsoft.Extensions.Options;
using System.Text.Json;
using SWS.BusinessObjects.AppSettings;
using SWS.BusinessObjects.Dtos;
using SWS.Services.ApiModels;
using AutoMapper;

namespace SWS.Services.Services.WarehouseAuthentication
{
    public class GoogleLoginService : IGoogleLoginService
    {
        private readonly GoogleAuthSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public GoogleLoginService(
            IOptions<GoogleAuthSettings> options,
            IMapper mapper)
        {
            _settings = options.Value;
            _httpClient = new HttpClient();
            _mapper = mapper;
        }

        public GoogleAuthUrlDto GetGoogleLoginUrl()
        {
            var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth" +
                $"?client_id={_settings.ClientId}" +
                $"&redirect_uri={_settings.RedirectUri}" +
                $"&response_type=code" +
                $"&scope=openid%20email%20profile" +
                $"&access_type=offline" +
                $"&prompt=consent";

            return new GoogleAuthUrlDto { AuthUrl = authUrl };
        }

        public async Task<GoogleUserInfoDto> GetUserInfoFromCodeAsync(string code)
        {
            // Exchange code for access token
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token")
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("client_id", _settings.ClientId),
                    new KeyValuePair<string, string>("client_secret", _settings.ClientSecret),
                    new KeyValuePair<string, string>("redirect_uri", _settings.RedirectUri),
                    new KeyValuePair<string, string>("grant_type", "authorization_code")
                })
            };
            
            var tokenResponse = await _httpClient.SendAsync(tokenRequest);
            if (!tokenResponse.IsSuccessStatusCode)
            {
                var errorContent = await tokenResponse.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Google token request failed: {tokenResponse.StatusCode} - {errorContent}");
            }
            
            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            using var tokenDoc = JsonDocument.Parse(tokenJson);
            var accessToken = tokenDoc.RootElement.GetProperty("access_token").GetString();

            // Get user info from Google API
            var userInfoResponse = await _httpClient.GetAsync($"https://www.googleapis.com/oauth2/v2/userinfo?access_token={accessToken}");
            userInfoResponse.EnsureSuccessStatusCode();
            
            var userInfoJson = await userInfoResponse.Content.ReadAsStringAsync();
            var googleUserInfo = JsonSerializer.Deserialize<GoogleUserInfo>(userInfoJson);
            
            // Map từ ApiModel sang DTO
            return _mapper.Map<GoogleUserInfoDto>(googleUserInfo) ?? new GoogleUserInfoDto();
        }
    }
}
