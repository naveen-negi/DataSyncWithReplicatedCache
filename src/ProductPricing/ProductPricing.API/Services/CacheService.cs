using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Newtonsoft.Json;

namespace ProductPricing.API.Services;

public interface ICacheService
{
    public User Get(string name);
}

[Serializable]
public class User
{
    public User(Guid Id, string Name)
    {
        this.Id = Id;
        this.Name = Name;
    }

    public Guid Id { get; }
    public string Name { get; }
}

public class CacheService : ICacheService
{
    private readonly ILogger<CacheService> _logger;
    private readonly ICache<string, string> _cache;

    public CacheService(IIgnite ignite, ILogger<CacheService> logger)
    {
        _logger = logger;
        _cache = ignite.GetCache<string, string>("ReplicatedCache");
        _logger.LogInformation("CacheService initialized");
        var user = _cache.Get("John");
        _logger.LogInformation("**************************************************");
        _logger.LogInformation("found a user in the cache {User}", user);
        _logger.LogInformation("**************************************************");
    }
    
    public User Get(string name)
    {
        var user = _cache.Get(name);
        return JsonConvert.DeserializeObject<User>(user);
    }
}