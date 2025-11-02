using SWS.ApiCore.Extensions;
using AppBackend.Extensions;
using SWS.BusinessObjects.AppSettings;
using SWS.BusinessObjects.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configs
builder.Services.AddCloudinaryConfig(builder.Configuration);
builder.Services.AddAutoMapperConfig();
builder.Services.AddDbConfig(builder.Configuration);
builder.Services.AddCorsConfig();
builder.Services.AddSwaggerConfig();
builder.Services.AddDefaultAuth(builder.Configuration);
//Optional login with google
// builder.Services.AddGoogleAuth(builder.Configuration);
builder.Services.AddSessionConfig();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRateLimitConfig();   
builder.Services.AddGeminiConfig(builder.Configuration);
builder.Services.AddWhisperConfig(builder.Configuration);

// Memory Cache for room locking
builder.Services.AddMemoryCache();

// All Application Services (includes Booking, Queue, Cache, etc.)
builder.Services.AddServicesConfig();

builder.Services.AddControllers()   
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("GoogleAuthSettings"));

var app = builder.Build();

// Seed database khi app khởi động
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await services.SeedDatabaseAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi khi seed database");
    }
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();   
// app.UseHttpsRedirection(); // Disabled for local development to avoid HTTPS port issues
app.UseCors("AllowAllOrigins");
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();