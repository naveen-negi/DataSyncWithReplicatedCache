using System.Diagnostics;

namespace Sessions.API.Controllers;

public interface  ISessionService
{
    // public Session Start(SessionStartRequest session);
    public Task<SessionResult> Stop(SessionEndRequest request);
    SessionResult UpdatePaymentDetails(PaymentDetailsRequest request);
    SessionResult Start(SessionStartRequest sessionStartRequest);
    SessionEntity Rollback(string sessionId, SessionRollbackRequest request);
}

public class PricingCalculation { }