using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWS.BusinessObjects.AppSettings;
using SWS.Services.Services.WhisperServices;
using System;

namespace SWS.ApiCore.Extensions
{
    public static class WhisperConfig
    {
        public static IServiceCollection AddWhisperConfig(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<WhisperSettings>(config.GetSection("Whisper"));

            services.AddHttpClient("whisper", client =>
            {
                var settings = config.GetSection("Whisper").Get<WhisperSettings>();
                client.BaseAddress = new Uri(settings?.BaseUrl ?? "http://127.0.0.1:8001");
                client.Timeout = TimeSpan.FromMinutes(settings?.TimeoutMinutes ?? 5);
            });

            services.AddScoped<IWhisperService, WhisperService>();

            return services;
        }
    }
}

