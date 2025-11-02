using SWS.BusinessObjects.Models;
using SWS.Repositories.Generic;
using SWS.Repositories.UnitOfWork;
using SWS.Services;
using SWS.Services.Helpers;
using SWS.Services.RateLimiting;
using SWS.Services.Services.Email;
using SWS.Services.Services.ProductServices;
using SWS.Services.Services.WarehouseAuthentication;
using SWS.Services.ConvertSqlRawServices;
using SWS.Services.Services.WhisperServices;
using SWS.Services.Services.ConvertSqlRawServices;

namespace SWS.ApiCore.Extensions;

public static class ServicesConfig
{
    public static IServiceCollection AddServicesConfig(this IServiceCollection services)
    {
        #region Generic Repository
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IDapperRepository, DapperRepository>();
        #endregion

        #region UnitOfWork
        services.AddScoped<IUnitOfWork>(provider =>
        {
            var context = provider.GetRequiredService<SmartWarehouseDbContext>();
            return new UnitOfWork(context);
        });
        #endregion

        #region Services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();
        services.AddScoped<IWarehouseAuthenticationService, WarehouseAuthenticationService>();
        services.AddScoped<IGoogleLoginService, GoogleLoginService>();
        services.AddScoped<IWarehouseProductService, WarehouseProductService>();
        services.AddSingleton<RateLimiterStore>();
        services.AddScoped<ITextToSqlService, TextToSqlService_Gemini>();
        services.AddScoped<IWhisperService, WhisperService>();
        #endregion

        #region Helpers
        services.AddScoped<CacheHelper>();
        #endregion

        return services;
    }
}