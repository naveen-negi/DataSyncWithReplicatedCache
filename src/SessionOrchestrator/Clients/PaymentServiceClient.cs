namespace SessionOrchestrator.Services;

public class PaymentServiceClient
{
    public PaymentResult ChargeCustomer(PricingCalculation sessionWithPrice)
    {
        return new PaymentResult();
    }
}

public record PaymentResult { }