using MediatR;
using Payments.API.Controllers;
using Payments.API.EventHandlers.Events;

namespace Payments.API.Services;

public interface IPaymentsService
{
    Task ChargeCustomer(BilledSessionRequest billedSession, CancellationToken ct = default);
}

public class PaymentsService : IPaymentsService
{
    private readonly ILogger<PaymentsService> _logger;
    private readonly IMediator _mediator;

    public PaymentsService( ILogger<PaymentsService> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task ChargeCustomer(BilledSessionRequest billedSession, CancellationToken ct = default)
    {
        await _mediator.Send(new SessionPriced(billedSession.SessionId, billedSession.UserId, billedSession.PriceAfterTax,
            billedSession.TaxAmount, billedSession.TaxBasisPoints, billedSession.Currency), ct);
    }
}