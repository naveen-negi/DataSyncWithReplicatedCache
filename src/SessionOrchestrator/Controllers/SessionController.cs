using Microsoft.AspNetCore.Mvc;
using SessionOrchestrator.Controllers.Dto;
using SessionOrchestrator.Services;
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
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while processing payment for session: " + e.Message);
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
            _logger.LogError(e, "Error while starting session: " + e.Message);
            return StatusCode(500);
        }
    }

}
