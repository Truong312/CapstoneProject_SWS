using SWS.ApiCore.Extensions;
using AppBackend.Extensions;
using SWS.BusinessObjects.AppSettings;

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

// Memory Cache for room locking
builder.Services.AddMemoryCache();

// All Application Services (includes Booking, Queue, Cache, etc.)
builder.Services.AddServicesConfig();

builder.Services.AddControllers()   
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("GoogleAuth"));

var app = builder.Build();

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