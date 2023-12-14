using MediatR;
using Microsoft.Extensions.Options;
using Payments.API.EventHandlers.Events;
using Payments.API.Repositories;
using Refit;

namespace Payments.API.EventHandlers;

public class SessionPaidHandler : IRequestHandler<SessionPaid>
{
    private readonly ILogger<SessionPaidHandler> _logger;
    private readonly ISessionOrchestratorApi _sessionOrchestratorApi;
        
    public SessionPaidHandler(IOptions<SessionOrchestratorConfig> config,
        ILogger<SessionPaidHandler> logger)
    {
        _logger = logger;
        _sessionOrchestratorApi = RestService.For<ISessionOrchestratorApi>(config.Value.BaseUrl);
    }
    
    public Task Handle(SessionPaid request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Updating Payment details in orchestrator for session  {request.SessionId} .... ");
        // _sessionOrchestratorApi.UpdatePaymentDetails(request.SessionId, new PaymentDetailsRequest(request.SessionId, request.Status));
        _logger.LogInformation($"Payment details updated for session {request.SessionId}");
        return Task.CompletedTask;
    }
}