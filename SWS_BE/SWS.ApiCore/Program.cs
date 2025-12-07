using System;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;

using SWS.Repositories.UnitOfWork;

using SWS.ApiCore.Extensions;
using SWS.BusinessObjects.AppSettings;
using SWS.BusinessObjects.Extensions;
// Import Orders
using SWS.Repositories.Repositories.ImportOrders;
// Return lookups / orders
using SWS.Repositories.Repositories.ReturnRepo;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ImportOrders;
using SWS.Services.ReturnLookups;
using SWS.Services.ReturnOrders;
using SWS.Services.Services.InventoryServices;
using SWS.Services.Services.ProductServices;
using System.Text.Json.Serialization;
using AppBackend.Extensions;
using SWS.ApiCore.Converters;

var builder = WebApplication.CreateBuilder(args);

// ====== IMPORTANT: logging safety MUST be immediately after CreateBuilder ======
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ===== DEBUG: in ra cấu hình JWT đang nạp =====
var envName = builder.Environment.EnvironmentName;
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
Console.WriteLine($"[BOOT] ENV={envName}, Jwt:KeyLen={(jwtKey?.Length ?? 0)}, Issuer={jwtIssuer}, Audience={jwtAudience}");

// ================== Services & Configs ==================
builder.Services.AddCloudinaryConfig(builder.Configuration);
builder.Services.AddAutoMapperConfig();
builder.Services.AddDbConfig(builder.Configuration);       // DbContext (SQL Server)
builder.Services.AddCorsConfig();                          // policy "AllowAllOrigins"
builder.Services.AddSwaggerConfig();                       // Swagger cơ bản
builder.Services.AddDefaultAuth(builder.Configuration);    // JWT auth

// Optional: Google
// builder.Services.AddGoogleAuth(builder.Configuration);

builder.Services.AddSessionConfig();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRateLimitConfig();
builder.Services.AddGeminiConfig(builder.Configuration);
builder.Services.AddWhisperConfig(builder.Configuration);

// Memory Cache for room locking
builder.Services.AddMemoryCache();
builder.Services.AddServicesConfig();

// Controllers + Json options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonDateOnlyConverter());
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Bind app settings
builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("GoogleAuthSettings"));

// ================== DI (Unit of Work & Repos/Services) ==================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Return: Lookups + Query
builder.Services.AddScoped<IReturnReasonRepository, ReturnReasonRepository>();
builder.Services.AddScoped<IReturnStatusRepository, ReturnStatusRepository>();
builder.Services.AddScoped<IReturnOrderQueryRepository, ReturnOrderQueryRepository>();
builder.Services.AddScoped<IReturnLookupService, ReturnLookupService>();
builder.Services.AddScoped<IReturnOrderQueryService, ReturnOrderQueryService>();

// ➕ Return: Command (Review)
builder.Services.AddScoped<IReturnOrderCommandRepository, ReturnOrderCommandRepository>();
builder.Services.AddScoped<IReturnOrderReviewService, ReturnOrderReviewService>();

// Import Orders
builder.Services.AddScoped<IImportOrderQueryRepository, ImportOrderQueryRepository>();
builder.Services.AddScoped<IImportOrderCommandRepository, ImportOrderCommandRepository>();
builder.Services.AddScoped<IImportOrderQueryService, ImportOrderQueryService>();
builder.Services.AddScoped<IImportOrderCommandService, ImportOrderCommandService>();
builder.Services.AddScoped<IInventoryDashboardService, InventoryDashboardService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

// ========== Authorization Policies (dựa trên Role) ==========
builder.Services.AddAuthorization(options =>
{
    // Chỉ Staff (role = "1") được phép tạo Import Order
    options.AddPolicy("StaffOnly", policy => policy.RequireRole("1"));

    // Staff hoặc Manager (role = "1" hoặc "2") được xem danh sách/chi tiết
    options.AddPolicy("StaffOrManager", policy => policy.RequireRole("1", "2"));

    // ➕ Manager duyệt trả hàng
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("2"));
});

// ================== Configure Kestrel URL: revert to original localhost:8080 ==================
builder.WebHost.UseUrls("http://localhost:8080");

// ================== Build App ==================
var app = builder.Build();

// Seed database khi app khởi động (safe logging fallback)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await services.SeedDatabaseAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetService<ILogger<Program>>();
        if (logger != null)
        {
            try
            {
                logger.LogError(ex, "Lỗi khi seed database");
            }
            catch (Exception logEx)
            {
                Console.WriteLine("Logger failed while logging the exception: " + logEx);
                Console.WriteLine("Original exception during seeding: " + ex);
            }
        }
        else
        {
            Console.WriteLine("No ILogger<Program> registered. Original exception during seeding:");
            Console.WriteLine(ex);
        }
    }
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();
app.UseCors("AllowAllOrigins");

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
