using ProductPricing.API.Dtos;

namespace ProductPricing.API.Entities;

public class Tariff
{
    public Guid Id { get; private set; }
    public DateTime ValidFrom { get; private set; }
    public DateTime ValidTo { get; private set; }
    public int PricePerHour { get; private set;  }
    public string LocationId { get; private set;   }
    public int TaxBasisPoints { get; private set;   }

    public Tariff( DateTime validFrom, DateTime validTo, int pricePerHour, string locationId, int taxBasisPoints)
    {
        Id = Guid.NewGuid();
        ValidFrom = validFrom;
        ValidTo = validTo;
        PricePerHour = pricePerHour;
        LocationId = locationId;
        TaxBasisPoints = taxBasisPoints;
    }
    
    public SessionPrice CalculatePrice(SessionPricingRequest session)
    {
        var hours = session.End.Subtract(session.Start).Hours;
        var price = (decimal) PricePerHour * hours;
        var taxAmount = price * TaxBasisPoints / 10000;
        var priceAfterTax = price + taxAmount;
        return new SessionPrice(price, priceAfterTax, TaxBasisPoints, taxAmount);
    }
}