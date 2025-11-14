using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWS.BusinessObjects.AppSettings;
using System;

namespace SWS.ApiCore.Extensions
{
    public static class GeminiAppConfig
    {
        public static IServiceCollection AddGeminiConfig(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<GeminiSettings>(config.GetSection("Gemini"));

            services.AddHttpClient("gemini", client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            return services;
        }
    }
}