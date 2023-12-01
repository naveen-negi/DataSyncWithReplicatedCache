using System.Text.Json.Serialization;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Sessions.API;
using Sessions.API.Controllers;
using Sessions.API.Entities;
using Sessions.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
{
    builder.Services.AddDbContext<SessionDBContext>();
    builder.Services.AddScoped<ISessionsRepository, SessionsRepository>();
    builder.Services.AddScoped<ISessionService, SessionService>();
    
    var configuration = builder.Configuration;
    
    builder.Services.Configure<ProductPricingServiceConfig>(configuration.GetSection("ProductPricingService"));
}

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenTelemetry()
    .WithTracing(b => b
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Sessions"))
    .AddAspNetCoreInstrumentation()
    .AddHttpClientInstrumentation()
    .AddJaegerExporter(options =>
    {
        options.AgentHost = "jaeger"; // Docker service name for Jaeger
        options.AgentPort = 6831;     // Default Jaeger agent UDP port
    }));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHttpLogging();

app.Run();
