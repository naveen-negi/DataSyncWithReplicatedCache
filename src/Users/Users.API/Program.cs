using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Users.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<ICacheService, CacheService>();

var ignite = Ignition.Start(new IgniteConfiguration
{
    MetricsLogFrequency = TimeSpan.Zero,
    CacheConfiguration = new[]
    {
        new CacheConfiguration
        {
            Name = "ReplicatedCache",
            CacheMode = CacheMode.Replicated
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

// Add Ignite instance to services
builder.Services.AddSingleton(ignite);

var app = builder.Build();

CacheService.InitCache(ignite);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();