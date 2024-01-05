using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Users.API.Entities;

namespace Users.API.Services;

public interface ICacheService { }

public class CacheService : ICacheService
{
    private readonly ICache<string,User> _cache;

    public CacheService(IIgnite ignite)
    {
        _cache = ignite.GetCache<string, User>("ReplicatedCache");
        var userId = Guid.NewGuid();
        var user = new User(userId, "John");
        _cache.Put(user.Name, user);
    }
}