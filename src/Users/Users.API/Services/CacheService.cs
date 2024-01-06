using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Newtonsoft.Json;
using Users.API.Entities;

namespace Users.API.Services;

public interface ICacheService
{
    public Object Get(string name);
}

public class CacheService : ICacheService
{
    private readonly ICache<string,string> _cache;

    public CacheService(IIgnite ignite)
    {
        _cache = ignite.GetCache<string, string>("ReplicatedCache");
        var userId = Guid.NewGuid();
        var user = new User(userId, "John");
        _cache.Put(user.Name, JsonConvert.SerializeObject(user));
    }
    
    public Object Get(string name)
    {
        return _cache.Get(name);
    }
}