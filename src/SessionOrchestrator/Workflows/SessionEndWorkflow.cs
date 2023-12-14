using Microsoft.Extensions.Options;
using ProductPricing.API;
using Refit;
using SessionOrchestrator.Clients;
using SessionOrchestrator.Controllers;
using SessionOrchestrator.Controllers.Dto;
using Stateless;

namespace SessionOrchestrator.Workflows;

enum WorkflowState {
    INIT, SESSIONS_STARTED, SESSION_PRICED, SESSION_PAID, SESSION_COMPLETED, SESSION_FAILED,
    SESSION_ENDED
}

public interface ISessionWorkflow
{
    Task StartSession(SessionStartRequest request);
    Task StopSession(string sessionId);
    void HandleSessionUpdate(SessionUpdateRequest request);
    Task HandlePriceUpdate(PricingUpdateRequest request);
    Task HandlePaymentUpdate(PaymentDetailsRequest request);
}

public class SessionWorkflow : ISessionWorkflow
{
    private readonly ILogger<SessionWorkflow> _logger;
    private readonly ISessionServiceApi _sessionServiceApi;
    private StateMachine<WorkflowState, Trigger> _machine;
    private readonly IProductPricingServiceApi _productPricingServiceApi;
    private readonly IPaymentsServiceApi _paymentsServiceApi;


    public SessionWorkflow(IOptions<SessionServiceConfig> sessionConfig,
        IOptions<ProductPricingServiceConfig> pricingConfig,
        IOptions<PaymentsServiceConfig> paymentsConfig,
        ILogger<SessionWorkflow> logger)
    {
        _logger = logger;
        _sessionServiceApi = RestService.For<ISessionServiceApi>(sessionConfig.Value.BaseUrl);
        _productPricingServiceApi = RestService.For<IProductPricingServiceApi>(pricingConfig.Value.BaseUrl);
        _paymentsServiceApi = RestService.For<IPaymentsServiceApi>(paymentsConfig.Value.BaseUrl);
        SetupStateMachine();
    }

    private void SetupStateMachine()
    {
        _machine = new StateMachine<WorkflowState, Trigger>(WorkflowState.INIT);

        _machine.Configure(WorkflowState.INIT)
            .Permit(Trigger.StartSession, WorkflowState.SESSIONS_STARTED);

        _machine.Configure(WorkflowState.SESSIONS_STARTED)
            .Permit(Trigger.FinishedSession, WorkflowState.SESSION_ENDED)
            .Permit(Trigger.FailSession, WorkflowState.SESSION_FAILED);

        _machine.Configure(WorkflowState.SESSION_ENDED)
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
    }

    public void HandleSessionUpdate(SessionUpdateRequest request)
    {
        // TODO: async flow doesn't work without persisting state in the database
        _machine.Fire(Trigger.StartSession);
        _machine.Fire(Trigger.FinishedSession);
        _productPricingServiceApi.CalculateSessionPrice(new SessionPricingRequest(request.SessionId, request.Start, request.End, request.LocationId, request.UserId));
        
        // TODO: Once database is implemented, we need to update the session with the end date and status
    }

    public async Task HandlePriceUpdate(PricingUpdateRequest request)
    {
        await _machine.FireAsync(Trigger.StartSession);
        await _machine.FireAsync(Trigger.FinishedSession);
        await _machine.FireAsync(Trigger.PriceSession);
        // FIXME: UserId either needs to flow in whole transaction or should be stored in database (harding coding for now)
        await _paymentsServiceApi.ProcessPayment(new BilledSessionRequest(request.SessionId, "1", request.PriceAfterTax, request.TaxAmount, request.TaxBasisPoints));
    }

    public async Task HandlePaymentUpdate(PaymentDetailsRequest request)
    {
        await _machine.FireAsync(Trigger.StartSession);
        await _machine.FireAsync(Trigger.FinishedSession);
        await _machine.FireAsync(Trigger.PriceSession);
        await _machine.FireAsync(Trigger.PaySession);
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
        StartSession, PriceSession, PaySession, CompleteSession, FailSession,
        FinishedSession
    }
}