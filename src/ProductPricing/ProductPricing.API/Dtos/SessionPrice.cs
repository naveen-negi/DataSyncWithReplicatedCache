namespace ProductPricing.API.Entities;

public record SessionPrice(decimal Price, decimal PriceAfterTax, int TaxBasisPoints, decimal TaxAmount, string Currency = "EUR");