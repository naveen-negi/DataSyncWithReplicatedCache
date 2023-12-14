using MediatR;
using Microsoft.Extensions.Options;
using Payments.API.Controllers;
using Payments.API.Entities;
using Payments.API.EventHandlers.Events;
using Payments.API.Repositories;

namespace Payments.API.Services;

public interface IPaymentsService
{
    Task<Payment> ChargeCustomer(BilledSessionRequest billedSession, CancellationToken ct = default);
}

public class PaymentsService : IPaymentsService
{
    private readonly IStripeApiClient _stripeApiClient;
    private readonly ILogger<PaymentsService> _logger;
    private readonly IMediator _mediator;
    private readonly IUserPaymentDetailsRepository _userRepository;
    private readonly IPaymentRepository _paymentRepository;

    public PaymentsService(IUserPaymentDetailsRepository userRepository, IPaymentRepository paymentRepository,
        IStripeApiClient stripeApiClient,
        ILogger<PaymentsService> logger,
        IMediator mediator)
    {
        _stripeApiClient = stripeApiClient;
        _logger = logger;
        _mediator = mediator;
        _userRepository = userRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<Payment> ChargeCustomer(BilledSessionRequest billedSession, CancellationToken ct = default)
    {
        var user = _userRepository.GetUser(billedSession.UserId);
        
        if (user is null)
        {
            throw new Exception("User not found");
        }

        var request = StripeChargeRequest(billedSession, user);
        var paymentDetails = PaymentDetails(billedSession, user);

        var paymentResult = await _stripeApiClient.CreateChargeAsync(request);
        if (!paymentResult.Status.Equals("succeeded"))
        {
            _logger.LogError($"Payment failed for session {billedSession.SessionId}");
            _paymentRepository.Save(paymentDetails with
            {
                Status = PaymentStatus.Failed, TransactionId = paymentResult.TransactionId
            });
        }

        paymentDetails = paymentDetails with
        {
            Status = PaymentStatus.Paid, TransactionId = paymentResult.TransactionId
        };
        
        _logger.LogInformation($"Payment successful for session {billedSession.SessionId}");

        _paymentRepository.Save(paymentDetails);
        await _mediator.Send(new SessionPaid(paymentDetails.SessionId), ct);
        return paymentDetails;
    }

    private static Payment PaymentDetails(BilledSessionRequest billedSession, UserPaymentDetails userPaymentDetails)
    {
        var paymentDetails = new Payment
        {
            PaymentId = Guid.NewGuid(),
            StripeCustomerId = userPaymentDetails.StripeCustomerId,
            UserId = billedSession.UserId,
            SessionId = billedSession.SessionId,
            Amount = billedSession.PriceAfterTax,
            Currency = billedSession.Currency,
            Status = PaymentStatus.Pending
        };
        return paymentDetails;
    }

    private static StripeChargeRequest StripeChargeRequest(BilledSessionRequest billedSession, UserPaymentDetails user)
    {
        var request =
            new StripeChargeRequest(user.StripeCustomerId, billedSession.PriceAfterTax, billedSession.Currency);
        return request;
    }
}