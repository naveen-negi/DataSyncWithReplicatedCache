using Microsoft.AspNetCore.Mvc;
using Refit;
using SessionOrchestrator.Controllers.Dto;
using SessionOrchestrator.Transactions;
using static SessionOrchestrator.Transactions.Transaction;

namespace SessionOrchestrator.Clients;

[Saga(new[] { SESSION_STARTED, SESSION_STOPPED })]
public interface IProductPricingServiceApi
{
    [Post("/api/prices/calculate")]
    public Task CalculateSessionPrice([Body] SessionPricingRequest request);
}

public class SessionPricingRequest
{
    public SessionPricingRequest(string sessionId, DateTime start, DateTime end, string locationId, string UserId)
    {
        SessionId = sessionId;
        Start = start;
        End = end;
        LocationId = locationId;
        this.UserId = UserId;
    }

    public string SessionId { get; private set; }
    public DateTime Start { get; private set; }
    public DateTime End { get; private set; }
    public string LocationId { get; private set; }
    public string UserId { get; private set; }

    public override string ToString()
    {
        return $"SessionId: {SessionId}, Start: {Start}, End: {End}, LocationId: {LocationId}";
    }
}