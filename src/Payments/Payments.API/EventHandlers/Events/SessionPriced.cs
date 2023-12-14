using MediatR;
using Payments.API.Entities;

namespace Payments.API.EventHandlers.Events;

public class SessionPriced : IRequest<Payment>
{
    public string SessionId { get; }
    public string UserId { get; }
    public decimal PriceAfterTax { get; }
    public decimal TaxAmount { get; }
    public int TaxBasisPoints { get; }
    public string Currency { get; }
    
    public SessionPriced( string sessionId, string userId, decimal priceAfterTax, decimal taxAmount, int taxBasisPoints, string currency = "EUR")
    {
        SessionId = sessionId;
        UserId = userId;
        PriceAfterTax = priceAfterTax;
        TaxAmount = taxAmount;
        TaxBasisPoints = taxBasisPoints;
        Currency = currency;
    }
}