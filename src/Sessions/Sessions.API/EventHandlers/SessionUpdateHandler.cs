using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Options;
using Refit;
using Sessions.API.EventHandlers.Events;
using Sessions.API.Repositories;

namespace Sessions.API.EventHandlers;

public class SessionUpdateHandler : IRequestHandler<SessionFinished>
{
    private readonly ISessionOrchestratorApi _sessionOrchestratorApi;
    private readonly ILogger<SessionUpdateHandler> _logger;

    public SessionUpdateHandler(IOptions<SessionOrchestratorConfig> options, ILogger<SessionUpdateHandler> logger)
    {
        _sessionOrchestratorApi = RestService.For<ISessionOrchestratorApi>(options.Value.BaseUrl);
        _logger = logger;
    }

    public async Task Handle(SessionFinished request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Session {request.SessionId} finished. Updating Orchestrator.");
        Thread.Sleep(1000);
        Debug.Assert(request.End != null, "request.End != null");
        await _sessionOrchestratorApi.Update(request.SessionId, new SessionUpdateRequest(request.SessionId,
            request.Status,
            request.UserId, request.LocationId, request.Start, request.End.Value));
        _logger.LogInformation($"Session {request.SessionId} updated. Orchestrator notified.");
    }
}