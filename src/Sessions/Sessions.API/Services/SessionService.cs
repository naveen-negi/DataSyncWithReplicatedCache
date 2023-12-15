using MediatR;
using Microsoft.Extensions.Options;
using Refit;
using Sessions.API.Controllers;
using Sessions.API.EventHandlers.Events;
using Sessions.API.Repositories;

namespace Sessions.API.Services;

class SessionService : ISessionService
{
    private ILogger<SessionService> _logger;
    private readonly IMediator _mediator;
    private readonly ISessionsRepository _sessionsRepository;
    private readonly ISessionOrchestratorApi _productPriceServiceApi;

    public SessionService(ISessionsRepository sessionsRepository,
        IOptions<ProductPricingServiceConfig> productPricingServiceConfig,
        ILogger<SessionService> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
        _sessionsRepository = sessionsRepository;
        _productPriceServiceApi = RestService.For<ISessionOrchestratorApi>(productPricingServiceConfig.Value.BaseUrl);
    }

    public async Task<SessionResult> Stop(SessionEndRequest request)
    {
        var session = _sessionsRepository.Get(Guid.Parse(request.SessionId));
        var stoppedSession = _sessionsRepository.Save(session.Stop());
        _logger.LogInformation(stoppedSession.ToString());

        // This could be retried.
        // But retry should be done in Handler
        // also Orchestrator should be idempotent (it is)
        await _mediator.Send(new SessionFinished(stoppedSession.Id.ToString(),
            stoppedSession.UserId,
            stoppedSession.LocationId,
            stoppedSession.StartDate,
            stoppedSession.EndDate));

        return new SessionResult(stoppedSession.Status, stoppedSession.Id, stoppedSession.UserId,
            stoppedSession.LocationId, stoppedSession.StartDate, stoppedSession.EndDate);
    }

    public SessionResult UpdatePaymentDetails(PaymentDetailsRequest request)
    {
        var session = _sessionsRepository.Get(Guid.Parse((ReadOnlySpan<char>)request.SessionId));
        session = session.UpdatePaymentDetails(request.PaymentDetails);
        return new SessionResult(session.Status, session.Id, session.UserId, session.LocationId, session.StartDate,
            session.EndDate);
    }

    public SessionResult Start(SessionStartRequest sessionStartRequest)
    {
        var session = new SessionEntity(sessionStartRequest.LocationId, sessionStartRequest.UserId);
        _sessionsRepository.Save(session);
        return new SessionResult(session.Status, session.Id, session.UserId, session.LocationId, session.StartDate,
            session.EndDate);
    }

    public SessionEntity Rollback(string sessionId, SessionRollbackRequest request)
    {
        return _sessionsRepository.Save(_sessionsRepository.Get(Guid.Parse(sessionId)).Rollback());
    }
}