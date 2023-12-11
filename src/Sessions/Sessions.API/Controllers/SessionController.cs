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
        public SessionResult Start([FromBody] SessionStartRequest request)
        {
            return _sessionService.Start(request);
        }
        
        [HttpPost("sessions/{sessionId}/rollback")]
        public SessionEntity Rollback(string sessionId, [FromBody] SessionRollbackRequest request)
        {
            _logger.LogInformation("Rolling back session {sessionId} with reason {reason}", sessionId, request.Reason);
            // return _sessionService.Rollback(sessionId, request);
            return new SessionEntity("locationId", "userId");
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

public record SessionStartRequest(string LocationId, string UserId);

public record SessionResult(SessionStatus Status, Guid SessionId, string UserId, string LocationId, DateTime StartDate, DateTime? EndDate);