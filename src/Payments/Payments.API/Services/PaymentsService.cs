using Microsoft.Extensions.Options;
using Payments.API.Controllers;
using Payments.API.Entities;
using Payments.API.Repositories;

namespace Payments.API.Services;

public interface IPaymentsService
{
    Task<Payment> ChargeCustomer(BilledSessionRequest billedSession, CancellationToken ct = default);
}

public class PaymentsService : IPaymentsService
{
    private readonly IStripeApiClient _stripeApiClient;
    private readonly IUserPaymentDetailsRepository _userRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ISessionApiClient _sessionApiClient;
    private readonly IProductPricingApiClient _productPricingApiClient;

    public PaymentsService(IUserPaymentDetailsRepository userRepository, IPaymentRepository paymentRepository,
        IStripeApiClient stripeApiClient,
        IOptions<ProductPricingServiceConfig> productPricingServiceConfig,
        IOptions<SessionServiceConfig> sessionServiceConfig)
    {
        _stripeApiClient = stripeApiClient;
        _userRepository = userRepository;
        _paymentRepository = paymentRepository;
        _productPricingApiClient = Refit.RestService.For<IProductPricingApiClient>(productPricingServiceConfig.Value.BaseUrl);
        _sessionApiClient = Refit.RestService.For<ISessionApiClient>(sessionServiceConfig.Value.BaseUrl); 
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
            _paymentRepository.Save(paymentDetails with
            {
                Status = PaymentStatus.Failed, TransactionId = paymentResult.TransactionId
            });

            // Now call two services 
            // First Session service to rollback the session
            // Since this is an atomic saga, we will need to add a compensation step
            // that will rollback the session to its original state
            // User will see an error and will retry to end the session again. 
            // This mean we need to roll back both the pricing and the session. 
            
            //TODO: What is the best way to run multiple services ?
            
            var session = await _sessionApiClient.RollbackSession(billedSession.SessionId,
                new SessionRollbackRequest( billedSession.SessionId, "Payment failed"));
            // await _productPricingApiClient.RollbackPricing(billedSession.SessionId,
            //     new PricingRollbackRequest("Payment failed"));
            
            //Should we throw and exception, even when the session is rolled back ?
            // But Customer should know that there was an error. 
            // But if we are propogating error back to the first caller, than first caller can rollback the session
            // why do we need to call the session service to rollback the session ?
            // throw new Exception("Payment failed");
        }

        paymentDetails = paymentDetails with
        {
            Status = PaymentStatus.Paid, TransactionId = paymentResult.TransactionId
        };

        _paymentRepository.Save(paymentDetails);
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