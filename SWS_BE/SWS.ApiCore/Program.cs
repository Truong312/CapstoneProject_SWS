using SWS.ApiCore.Extensions;
using AppBackend.Extensions;
using SWS.BusinessObjects.AppSettings;
using SWS.Repositories.Repositories.ReturnRepo;
using SWS.Services.ReturnLookups;
using SWS.Services.ReturnOrders;
using SWS.Services.Services.ProductServices;

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
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
    });

builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("GoogleAuthSettings"));
builder.Services.AddScoped<IReturnReasonRepository, ReturnReasonRepository>();
builder.Services.AddScoped<IReturnStatusRepository, ReturnStatusRepository>();
builder.Services.AddScoped<IReturnOrderQueryRepository, ReturnOrderQueryRepository>();

builder.Services.AddScoped<IReturnLookupService, ReturnLookupService>();
builder.Services.AddScoped<IReturnOrderQueryService, ReturnOrderQueryService>();
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