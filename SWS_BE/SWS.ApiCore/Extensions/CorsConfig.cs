using Microsoft.Extensions.DependencyInjection;

namespace SWS.ApiCore.Extensions;

public static class CorsConfig
{
    public static IServiceCollection AddCorsConfig(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            // Permissive policy for development: allow any origin and support credentials.
            // NOTE: This is unsafe for production. In production restrict origins and do NOT use SetIsOriginAllowed(_ => true)
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.SetIsOriginAllowed(_ => true)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
        });
        return services;
    }
}
