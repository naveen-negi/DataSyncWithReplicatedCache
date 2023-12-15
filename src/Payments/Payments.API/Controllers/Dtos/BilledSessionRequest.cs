namespace Payments.API.Controllers;

public record BilledSessionRequest(string SessionId, string UserId, decimal PriceAfterTax, decimal TaxAmount,
    int TaxBasisPoints, string Currency = "EUR");