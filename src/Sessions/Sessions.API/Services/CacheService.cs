using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Newtonsoft.Json;

namespace Sessions.API.Services;

public interface ICacheService
{
    public User Get(string licensePlate);
}

[Serializable]
public class User
{
    public User(string id, string name, string licensePlate)
    {
        Id = id;
        Name = name;
        LicensePlate = licensePlate;
    }

    public string Id { get; }
    public string Name { get; }
    public string LicensePlate { get; }
}

public class CacheService : ICacheService
{
    private readonly ILogger<CacheService> _logger;
    private readonly ICache<string, string> _cache;

    public CacheService(IIgnite ignite, ILogger<CacheService> logger)
    {
        _logger = logger;
        _cache = ignite.GetCache<string, string>("ReplicatedCache");
    }
    
    public User Get(string licensePlate)
    {
        var user = _cache.Get(licensePlate);
        return JsonConvert.DeserializeObject<User>(user);
    }
}