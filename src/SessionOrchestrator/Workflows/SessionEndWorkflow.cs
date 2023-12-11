using Microsoft.Extensions.Options;
using ProductPricing.API;
using Refit;
using SessionOrchestrator.Clients;
using SessionOrchestrator.Controllers.Dto;
using Stateless;

namespace SessionOrchestrator.Workflows;

enum WorkflowState {
    INIT, SESSIONS_STARTED, SESSION_PRICED, SESSION_PAID, SESSION_COMPLETED, SESSION_FAILED
}

public interface ISessionWorkflow
{
    Task StartSession(SessionStartRequest request);
    Task StopSession(string sessionId);
}

public class SessionWorkflow : ISessionWorkflow
{
    private readonly ILogger<SessionWorkflow> _logger;
    private readonly ISessionServiceApi _sessionServiceApi;
    private StateMachine<WorkflowState, Trigger> _machine;
    

    public SessionWorkflow(IOptions<SessionServiceConfig> options, ILogger<SessionWorkflow> logger)
    {
        _logger = logger;
        _sessionServiceApi = RestService.For<ISessionServiceApi>(options.Value.BaseUrl);
        _logger.LogInformation("BaseUrl: " + options.Value.BaseUrl);
        SetupStateMachine();
    }

    private void SetupStateMachine()
    {
        _machine = new StateMachine<WorkflowState, Trigger>(WorkflowState.INIT);

        _machine.Configure(WorkflowState.INIT)
            .Permit(Trigger.StartSession, WorkflowState.SESSIONS_STARTED);

        _machine.Configure(WorkflowState.SESSIONS_STARTED)
            .Permit(Trigger.PriceSession, WorkflowState.SESSION_PRICED)
            .Permit(Trigger.FailSession, WorkflowState.SESSION_FAILED);

        _machine.Configure(WorkflowState.SESSION_PRICED)
            .Permit(Trigger.PaySession, WorkflowState.SESSION_PAID)
            .Permit(Trigger.FailSession, WorkflowState.SESSION_FAILED);

        _machine.Configure(WorkflowState.SESSION_PAID)
            .Permit(Trigger.CompleteSession, WorkflowState.SESSION_COMPLETED)
            .Permit(Trigger.FailSession, WorkflowState.SESSION_FAILED);
    }

    public async Task StartSession(SessionStartRequest request)
    {
        await _sessionServiceApi.StartSession(request);
        await _machine.FireAsync(Trigger.StartSession);
    }

    public async Task StopSession(string sessionId)
    {
        await _machine.FireAsync(Trigger.StartSession);
        await _sessionServiceApi.StopSession(sessionId);
        await _machine.FireAsync(Trigger.PriceSession);
    }

    public void HandleSessionBillingUpdate(SessionBillingUpdateRequest request)
    {
        _machine.Fire(Trigger.PaySession);
    }

    public void HandleSessionPaymentUpdate(SessionPaymentUpdateRequest request)
    {
        _machine.Fire(Trigger.CompleteSession);
    }

    public void FailSession() => _machine.Fire(Trigger.FailSession);

    enum Trigger
    {
        StartSession, PriceSession, PaySession, CompleteSession, FailSession
    }
}