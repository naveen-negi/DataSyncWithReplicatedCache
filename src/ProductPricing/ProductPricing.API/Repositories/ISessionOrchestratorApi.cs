using Refit;

namespace ProductPricing.API.Repositories;

public record PricingUpdateRequest(string SessionId, decimal Price, decimal PriceAfterTax, int TaxBasisPoints,
    decimal TaxAmount);

public interface ISessionOrchestratorApi
{
    [Post("/api/workflow/{sessionId}/price")]
    public Task Update(string sessionId, [Body] PricingUpdateRequest request);
}