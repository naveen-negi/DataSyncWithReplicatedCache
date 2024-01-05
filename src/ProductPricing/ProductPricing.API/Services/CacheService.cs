using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;

namespace ProductPricing.API.Services;

public interface ICacheService
{
    public User Get(string name);
}

public record User(Guid Id, string Name);

public class CacheService : ICacheService
{
    private readonly ILogger<CacheService> _logger;
    private readonly ICache<string,User> _cache;

    public CacheService(IIgnite ignite, ILogger<CacheService> logger)
    {
        _logger = logger;
        _cache = ignite.GetCache<string, User>("ReplicatedCache");
        _logger.LogInformation("CacheService initialized");
        var user = _cache.Get("John");
        _logger.LogInformation("**************************************************");
        _logger.LogInformation("found a user in the cache {User}", user);
        _logger.LogInformation("**************************************************");
    }
    
    public User Get(string name)
    {
        var user = _cache.Get(name);
        _logger.LogInformation("**************************************************");
        _logger.LogInformation("found a user in the cache {User}", user);
        _logger.LogInformation("**************************************************");
        return user;
    }
}