using System.Runtime.InteropServices.JavaScript;
using Refit;

namespace Sessions.API.Controllers;

public record SessionPricingRequest(string SessionId, DateTime Start, DateTime? End, string locationId, string UserId);

public interface IProductPricingApi
{
    [Post("/api/prices/calculate")]
    public Task<PricingCalculation> CalculatePrice([Body] SessionPricingRequest sessionEntity);
}