using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Models;

namespace AppBackend.Extensions;

public static class DbConfig
{
    public static IServiceCollection AddDbConfig(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<SmartWarehouseDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
        return services;
    }
}
