using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProductPricing.API;
using ProductPricing.API.Entities;
using ProductPricing.API.Repositories;
using ProductPricing.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
{
    builder.Services.AddDbContext<TariffDBContext>();
    builder.Services.AddScoped<ITariffRepository, TariffRepository>();
    builder.Services.AddScoped<ITariffService, TariffService>();
    
    var configuration = builder.Configuration;
    
    builder.Services.Configure<PaymentsServiceConfig>(configuration.GetSection("PaymentsService"));
    
    builder.Services.AddOpenTelemetry()
        .WithTracing(b => b
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("ProductPricing"))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddJaegerExporter(options =>
        {
            options.AgentHost = "jaeger"; // Docker service name for Jaeger
            options.AgentPort = 6831;     // Default Jaeger agent UDP port
        }));
}

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHttpLogging();

app.Run();
