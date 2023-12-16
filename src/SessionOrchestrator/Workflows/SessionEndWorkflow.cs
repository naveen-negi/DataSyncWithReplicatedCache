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
    Task HandleSessionUpdate(SessionUpdateRequest request);
    Task HandlePriceUpdate(PricingUpdateRequest request);
    Task HandlePaymentUpdate(PaymentDetailsRequest request);
}

public class SessionWorkflow : ISessionWorkflow
{
    private readonly ILogger<SessionWorkflow> _logger;
    private readonly ISessionWorkflowRepository _sessionWorkflowRepository;
    private readonly ISessionServiceApi _sessionServiceApi;
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
        var workflow = new SessionWorkflowEntity(response.SessionId, request.UserId, WorkflowState.SESSIONS_STARTED);
        await _sessionWorkflowRepository.SaveSessionWorkflow(workflow);
        _logger.LogInformation("Workflow state {workflowToDotGraph}", workflow.ToDotGraph());
        return response;
    }

    public async Task StopSession(string sessionId)
    {
        // TODO: think about if need a state for this ? like SESSION_STOPPED_REQUESTED
        await _sessionServiceApi.StopSession(sessionId);
    }

    public async Task HandleSessionUpdate(SessionUpdateRequest request)
    {
        var sessionWorkflow = await _sessionWorkflowRepository.GetSessionWorkflow(Guid.Parse(request.SessionId));
        // TODO: we need to update the session with the end date and status
        sessionWorkflow!.StopSession();
        await _productPricingServiceApi.CalculateSessionPrice(new SessionPricingRequest(request.SessionId, request.Start,
            request.End, request.LocationId, request.UserId));
        await _sessionWorkflowRepository.SaveSessionWorkflow(sessionWorkflow);
    }

    public async Task HandlePriceUpdate(PricingUpdateRequest request)
    {
        var sessionWorkflow = await _sessionWorkflowRepository.GetSessionWorkflow(request.SessionId);
        sessionWorkflow!.PriceSession();
        // FIXME: UserId either needs to flow in whole transaction or should be stored in database (harding coding for now)
        await _paymentsServiceApi.ProcessPayment(new BilledSessionRequest(request.SessionId.ToString(), "1", request.PriceAfterTax,
            request.TaxAmount, request.TaxBasisPoints));
        await _sessionWorkflowRepository.SaveSessionWorkflow(sessionWorkflow);
    }

    public async Task HandlePaymentUpdate(PaymentDetailsRequest request)
    {
        var sessionWorkflow = await _sessionWorkflowRepository.GetSessionWorkflow(request.SessionId);
        sessionWorkflow!.PaySession();
        await _sessionWorkflowRepository.SaveSessionWorkflow(sessionWorkflow);
    }
}