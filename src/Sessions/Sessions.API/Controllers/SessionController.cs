using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sessions.API.Controllers;

namespace Sessions.API.Controllers
{
    [Route("api")]
    public class SessionController : ControllerBase
    {
        private readonly ILogger<SessionController> _logger;

        private readonly ISessionService _sessionService;

        public SessionController(ILogger<SessionController> logger, ISessionService sessionService)
        {
            _logger = logger;
            _sessionService = sessionService;
        }

        [HttpPost("sessions")]
        public Task<SessionResult> Start([FromBody] SessionStartRequest request)
        {
            _logger.LogInformation($"Session start request received.Session for user: {request.UserId} started.");
            return Task.FromResult(_sessionService.Start(request));
        }

        [HttpPost("sessions/{sessionId}/end")]
        public async Task<IActionResult> End(string sessionId)
        {
            try
            {
                var result = await _sessionService.Stop(new SessionEndRequest(sessionId));
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while stopping session: " + e.Message);
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("sessions/{sessionId}/paymentDetails")]
        public SessionResult UpdatePaymentDetails(PaymentDetailsRequest request)
        {
            return _sessionService.UpdatePaymentDetails(request);
        }
    }

    public record SessionRollbackRequest(string SessionId, string Reason);
}

public record SessionEndRequest(string SessionId);

public record SessionStartRequest(string LocationId, string LicensePlate, string? UserId);

public record SessionResult(SessionStatus Status, Guid SessionId, string UserId, string LocationId, DateTime StartDate,
    DateTime? EndDate);