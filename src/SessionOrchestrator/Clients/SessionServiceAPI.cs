using Microsoft.AspNetCore.Mvc;
using Refit;
using SessionOrchestrator.Controllers.Dto;
using SessionOrchestrator.Transactions;
using static SessionOrchestrator.Transactions.Transaction;

namespace SessionOrchestrator.Clients;


public record SessionResult(string Status, Guid SessionId, string UserId, string LocationId, DateTime StartDate, DateTime? EndDate);

[Saga(new[] { SESSION_STARTED, SESSION_STOPPED })]
public interface ISessionServiceApi
{
    [Post("/api/sessions")]
    public Task StartSession([Body] SessionStartRequest request);
    [Post("/api/sessions/{sessionId}/end")]
    public Task<SessionResult> StopSession(string sessionId);
}