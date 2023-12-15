using Refit;

namespace Sessions.API.Repositories;

public record SessionUpdateRequest(string SessionId, SessionStatus Status,
    string UserId, string LocationId, DateTime Start, DateTime End);

public interface ISessionOrchestratorApi
{
    [Post("/api/sessions/{sessionId}/update")]
    public Task Update(string sessionId, [Body] SessionUpdateRequest request);
}