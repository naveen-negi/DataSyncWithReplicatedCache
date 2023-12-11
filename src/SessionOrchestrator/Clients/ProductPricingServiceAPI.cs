using SessionOrchestrator.Clients;

namespace SessionOrchestrator.Services;

public class ProductPricingServiceAPI
{
    public PricingCalculation CalculatePrice(SessionResult stoppedSession)
    {
        return new PricingCalculation();
    }
    
    public PricingCalculation RollBackPricingUpdate(SessionResult stoppedSession)
    {
        return new PricingCalculation();
    }
}

public record PricingCalculation { }