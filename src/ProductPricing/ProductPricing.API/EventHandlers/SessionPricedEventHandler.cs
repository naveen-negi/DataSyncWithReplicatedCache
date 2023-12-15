using MediatR;
using Microsoft.Extensions.Options;
using ProductPricing.API.EventHandlers.Events;
using ProductPricing.API.Repositories;
using Refit;

namespace ProductPricing.API.EventHandlers;

public class SessionPricedEventHandler : IRequestHandler<SessionPriced>
{
    private readonly ILogger<SessionPricedEventHandler> _logger;
    private readonly ISessionOrchestratorApi _sessionOrchestratorApi;

    public SessionPricedEventHandler(IOptions<SessionOrchestratorServiceConfig> config,
        ILogger<SessionPricedEventHandler> logger)
    {
        _logger = logger;
        _sessionOrchestratorApi = RestService.For<ISessionOrchestratorApi>(config.Value.BaseUrl);
    }

    public async Task Handle(SessionPriced request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Session {request.SessionId} priced. Updating Orchestrator.");
        var pricingUpdateRequest = new PricingUpdateRequest(request.SessionId, request.Price, request.PriceAfterTax,
            request.TaxBasisPoints, request.TaxAmount);
        await _sessionOrchestratorApi.Update(request.SessionId, pricingUpdateRequest);
        _logger.LogInformation($"Session {request.SessionId} updated. Orchestrator notified.");
    }
}