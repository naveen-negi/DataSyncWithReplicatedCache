using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Microsoft.EntityFrameworkCore;
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
    builder.Services.AddScoped<ICacheService, CacheService>();

    var configuration = builder.Configuration;

    builder.Services.Configure<PaymentsServiceConfig>(configuration.GetSection("PaymentsService"));
    builder.Services.Configure<SessionOrchestratorServiceConfig>(
        configuration.GetSection("SessionOrchestratorService"));

    builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly); });

    builder.Services.AddOpenTelemetry()
        .WithTracing(b => b
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("ProductPricing"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddJaegerExporter(options =>
            {
                options.AgentHost = "jaeger"; // Docker service name for Jaeger
                options.AgentPort = 6831; // Default Jaeger agent UDP port
            }));
}

builder.Services.AddControllers();

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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<TariffDBContext>();

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