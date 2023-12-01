using Sessions.API.Entities;

namespace Sessions.API.Controllers;

public interface ISessionsRepository
{
    public SessionEntity Save(SessionEntity sessionEntity);
    public SessionEntity Get(Guid id);
}

public class SessionsRepository : ISessionsRepository
{
    private readonly SessionDBContext _sessionDbContext;

    public SessionsRepository(SessionDBContext sessionDbContext)
    {
        _sessionDbContext = sessionDbContext;
    }
    
    public SessionEntity Save(SessionEntity sessionEntity)
    {
        // Check if the entity already exists in the database
        var existingEntity = _sessionDbContext.Sessions.Find(sessionEntity.Id); 

        if (existingEntity == null)
        {
            _sessionDbContext.Sessions.Add(sessionEntity);
        }
        else
        {
            _sessionDbContext.Entry(existingEntity).CurrentValues.SetValues(sessionEntity);
        }

        // Save changes to the database
        _sessionDbContext.SaveChanges();
        return sessionEntity;
    }


    public SessionEntity Get(Guid id)
    {
        return _sessionDbContext.Sessions.Find(id)!;
    }
}