using Microsoft.AspNetCore.Mvc;
using SessionOrchestrator.Controllers.Dto;
using SessionOrchestrator.Workflows;

namespace SessionOrchestrator.Controllers;

[Route("api")]
public class SessionController : ControllerBase
{
    private readonly ILogger<SessionController> _logger;
    private readonly ISessionWorkflow _workflow;

    public SessionController(ILogger<SessionController> logger, ISessionWorkflow workflow)
    {
        _logger = logger;
        _workflow = workflow;
    }

    [HttpPost("sessions/{sessionId}/processPayment")]
    public async Task<IActionResult> End(string sessionId)
    {
        try
        {
            await _workflow.StopSession(sessionId);
            return Ok(sessionId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while processing payment for session:  {Message}" , e.Message);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("workflow/startSession")]
    public async Task<IActionResult> Start([FromBody] SessionStartRequest request)
    {
        try
        {
            _logger.LogInformation("Session start request received.Session for user: {RequestUserId} started", request.licensePlate);
            var response = await _workflow.StartSession(request);
            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while starting session: {Message}" , e.Message);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("sessions/{sessionId}/update")]
    public async Task<IActionResult> UpdateSession([FromBody] SessionUpdateRequest request)
    {
        try
        {
            _logger.LogInformation("Session update request received.Session {RequestSessionId} updated", request.SessionId);
            await _workflow.HandleSessionUpdate(request);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating session data: {Message}" , e.Message);
            return StatusCode(500);
        }
    }

    [HttpPost("workflow/{sessionId}/price")]
    public async Task<IActionResult> UpdatePrice([FromBody] PricingUpdateRequest request)
    {
        try
        {
            _logger.LogInformation("Price update request received.Session {RequestSessionId} updated", request.SessionId);
            await _workflow.HandlePriceUpdate(request);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating Pricing data: {Message} " , e.Message);
            return StatusCode(500);
        }
    }

    [HttpPost("workflow/{sessionId}/payment")]
    public async Task<IActionResult> UpdatePaymentDetails([FromBody] PaymentDetailsRequest request)
    {
        try
        {
            _logger.LogInformation("Payment details update request received. Session {Id} updated", request.SessionId);
            await _workflow.HandlePaymentUpdate(request);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating Pricing data: {Message} " , e.Message);
            return StatusCode(500);
        }
    }


    [HttpPost("sessions/start")]
    public async Task<IActionResult> End(SessionStartRequest request)
    {
        try
        {
            await _workflow.StartSession(request);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating Pricing data: {Message} " , e.Message);
            return StatusCode(500);
        }
    }
}

public record PricingUpdateRequest(Guid SessionId, decimal Price, decimal PriceAfterTax, int TaxBasisPoints,
    decimal TaxAmount);

public record PaymentDetailsRequest(Guid SessionId, string Status);