using System.Text.Json.Serialization;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
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
    builder.Services.AddScoped<ICacheService, CacheService>();

    var configuration = builder.Configuration;

    builder.Services.Configure<ProductPricingServiceConfig>(configuration.GetSection("ProductPricingService"));
    builder.Services.Configure<SessionOrchestratorConfig>(configuration.GetSection("SessionOrchestrator"));
    builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly); });
}

builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields = HttpLoggingFields.All;
    o.RequestBodyLogLimit = 4096;
    o.ResponseBodyLogLimit = 4096;
});

builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

var ignite = Ignition.Start(new IgniteConfiguration
{
    CacheConfiguration = new[] 
    {
        new CacheConfiguration
        {
            Name = "ReplicatedCache",
            CacheMode = CacheMode.Replicated,
        }
    },
    DiscoverySpi = new TcpDiscoverySpi
    {
        IpFinder = new TcpDiscoveryStaticIpFinder
        {
            Endpoints = new[] { "users-api:47500", "productpricing-api:47600", "sessions-api:47700"}
        }
    }
});

builder.Services.AddSingleton(ignite);

builder.Services.AddOpenTelemetry()
    .WithTracing(b => b
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Sessions"))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddJaegerExporter(options =>
        {
            options.AgentHost = "jaeger"; // Docker service name for Jaeger
            options.AgentPort = 6831; // Default Jaeger agent UDP port
        }));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<SessionDBContext>();

    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Log or handle the exception as needed
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHttpLogging();

app.Run();