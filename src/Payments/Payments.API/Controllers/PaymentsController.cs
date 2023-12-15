using Microsoft.AspNetCore.Mvc;
using Payments.API.Entities;
using Payments.API.Services;

namespace Payments.API.Controllers;

[Route("api")]
public class PaymentsController : ControllerBase
{
    private readonly ILogger<PaymentsController> _logger;
    private readonly IPaymentsService _paymentsService;

    public PaymentsController(ILogger<PaymentsController> logger, IPaymentsService paymentsService)
    {
        _logger = logger;
        _paymentsService = paymentsService;
    }

    [HttpPost("payments/pay")]
    public async Task<IActionResult> ChargeCustomer([FromBody] BilledSessionRequest request)
    {
        try
        {
            _logger.LogInformation(
                $"Payment request received. Payment Processing started for Session {request.SessionId}.");
            await _paymentsService.ChargeCustomer(request);
            return Accepted();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while charging customer");
            return StatusCode(500, e.Message);
        }
    }
}