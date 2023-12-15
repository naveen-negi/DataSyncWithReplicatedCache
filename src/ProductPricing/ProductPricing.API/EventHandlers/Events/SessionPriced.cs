using MediatR;

namespace ProductPricing.API.EventHandlers.Events;

public class SessionPriced : IRequest
{
    public string SessionId { get; private set; }

    public decimal Price { get; private set; }
    public decimal PriceAfterTax { get; private set; }
    public int TaxBasisPoints { get; private set; }
    public decimal TaxAmount { get; private set; }

    public SessionPriced(string sessionId, decimal price, decimal priceAfterTax, int taxBasisPoints, decimal taxAmount)
    {
        SessionId = sessionId;
        Price = price;
        PriceAfterTax = priceAfterTax;
        TaxBasisPoints = taxBasisPoints;
        TaxAmount = taxAmount;
    }
}