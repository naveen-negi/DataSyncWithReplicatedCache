using Microsoft.Extensions.Options;
using ProductPricing.API;
using Refit;
using SessionOrchestrator.Clients;
using SessionOrchestrator.Controllers;
using SessionOrchestrator.Controllers.Dto;
using SessionOrchestrator.Entities;
using SessionOrchestrator.Repositories;
using Stateless;

namespace SessionOrchestrator.Workflows;


public interface ISessionWorkflow
{
    Task<sessionStartResponse> StartSession(SessionStartRequest request);
    Task StopSession(string sessionId);
    void HandleSessionUpdate(SessionUpdateRequest request);
    Task HandlePriceUpdate(PricingUpdateRequest request);
    Task HandlePaymentUpdate(PaymentDetailsRequest request);
}

public class SessionWorkflow : ISessionWorkflow
{
    private readonly ILogger<SessionWorkflow> _logger;
    private readonly ISessionWorkflowRepository _sessionWorkflowRepository;
    private readonly ISessionServiceApi _sessionServiceApi;
    private StateMachine<WorkflowState, Trigger> _machine;
    private readonly IProductPricingServiceApi _productPricingServiceApi;
    private readonly IPaymentsServiceApi _paymentsServiceApi;


    public SessionWorkflow(IOptions<SessionServiceConfig> sessionConfig,
        IOptions<ProductPricingServiceConfig> pricingConfig,
        IOptions<PaymentsServiceConfig> paymentsConfig,
        ILogger<SessionWorkflow> logger,
        ISessionWorkflowRepository sessionWorkflowRepository)
    {
        _logger = logger;
        _sessionWorkflowRepository = sessionWorkflowRepository;
        _sessionServiceApi = RestService.For<ISessionServiceApi>(sessionConfig.Value.BaseUrl);
        _productPricingServiceApi = RestService.For<IProductPricingServiceApi>(pricingConfig.Value.BaseUrl);
        _paymentsServiceApi = RestService.For<IPaymentsServiceApi>(paymentsConfig.Value.BaseUrl);
    }
    public async Task<sessionStartResponse> StartSession(SessionStartRequest request)
    {
        var response = await _sessionServiceApi.StartSession(request);
        _logger.LogInformation("Received session id {ResponseSessionId} from session service", response.SessionId);
        var workflow = new SessionWorkflowEntity(response.SessionId, request.UserId);
        await _sessionWorkflowRepository.SaveSessionWorkflow(workflow);
        _logger.LogInformation("Workflow state {workflowToDotGraph}", workflow.ToDotGraph());
        return response;
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
        _productPricingServiceApi.CalculateSessionPrice(new SessionPricingRequest(request.SessionId, request.Start,
            request.End, request.LocationId, request.UserId));

        // TODO: Once database is implemented, we need to update the session with the end date and status
    }

    public async Task HandlePriceUpdate(PricingUpdateRequest request)
    {
        await _machine.FireAsync(Trigger.StartSession);
        await _machine.FireAsync(Trigger.FinishedSession);
        await _machine.FireAsync(Trigger.PriceSession);
        // FIXME: UserId either needs to flow in whole transaction or should be stored in database (harding coding for now)
        await _paymentsServiceApi.ProcessPayment(new BilledSessionRequest(request.SessionId, "1", request.PriceAfterTax,
            request.TaxAmount, request.TaxBasisPoints));
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
}