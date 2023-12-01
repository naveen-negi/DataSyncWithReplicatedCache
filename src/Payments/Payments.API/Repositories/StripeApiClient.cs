namespace Payments.API.Repositories;

public interface IStripeApiClient
{
    Task<StripeChargeResponse> CreateChargeAsync(StripeChargeRequest request);
}

// Create a mock stripe client
// Alternatively, you can use docker image of stripe mock

public class MockStripeApiClient : IStripeApiClient
{
    public Task<StripeChargeResponse> CreateChargeAsync(StripeChargeRequest request)
    {
        return Task.FromResult(request.CustomerId == "cus_999" ?
            new StripeChargeResponse(Guid.NewGuid().ToString(), "failed") 
            : new StripeChargeResponse(Guid.NewGuid().ToString(), "succeeded"));
    }
}

public record StripeChargeRequest(string CustomerId, decimal Amount, string Currency = "EUR");

public record StripeChargeResponse(string TransactionId, string Status); 