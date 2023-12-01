using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Payments.API.Repositories;

public interface ISessionApiClient
{
    [Post("/api/sessions/{sessionId}/rollback")]
    Task<Session> RollbackSession([AliasAs("sessionId")] string sessionId, [Body] SessionRollbackRequest request);
}

public record SessionRollbackRequest(string SessionId, string Reason);

public record Session(string Status, string UserId);
