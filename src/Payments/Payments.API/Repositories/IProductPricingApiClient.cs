using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Payments.API.Repositories;

public interface IProductPricingApiClient
{
    [Post("/api/pricing/{sessionId}/rollback")]
    Task RollbackPricing([AliasAs("sessionId")] string sessionId, [Body] PricingRollbackRequest request);
    
}

public record PricingRollbackRequest(string Reason);