using SessionOrchestrator.Controllers;
using SessionOrchestrator.Transactions;
using static SessionOrchestrator.Transactions.Transaction;

namespace SessionOrchestrator.Services;

public interface ISessionServiceAPI
{
    public void StartSession();
    public SessionResult StopSession();
}

[Saga(new[] { SESSION_STARTED, SESSION_STOPPED })]
public class SessionServiceAPI 
{
   public void StartSession()
   {
   }

   public SessionResult StopSession(SessionEndRequest sessionEndRequest)
   {
       return new SessionResult();
   }
   
   public SessionResult RollBackStopSessionUpdate(SessionEndRequest sessionEndRequest)
   {
       return new SessionResult();
   }
}

public record SessionResult {};