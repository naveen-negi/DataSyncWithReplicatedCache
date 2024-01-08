using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Newtonsoft.Json;
using Users.API.Entities;

namespace Users.API.Services;

public interface ICacheService
{
    public string Get(string name);
    void Add(User user);
}

public class CacheService : ICacheService
{
    private ICache<string,string> _cache;

    public CacheService(IIgnite ignite)
    {
        _cache = ignite.GetCache<string, string>("ReplicatedCache");
    }

    public static void InitCache(IIgnite ignite)
    {
        var cache = ignite.GetCache<string, string>("ReplicatedCache");
        var user1 = new User("1", "John", "ABC123");
        cache.Put(user1.LicensePlate, JsonConvert.SerializeObject(user1));


        var user2 = new User("2", "Doe", "XYZ123");
        cache.Put(user2.LicensePlate, JsonConvert.SerializeObject(user2));
    }

    public string Get(string name)
    {
        return _cache.Get(name);
    }
    
    public void Add(User user)
    {
        _cache.GetAndPut(user.LicensePlate, JsonConvert.SerializeObject(user));
    }
}