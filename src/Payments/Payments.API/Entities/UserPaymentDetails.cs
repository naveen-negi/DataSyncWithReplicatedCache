namespace Payments.API.Entities;

public class UserPaymentDetails
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string StripeCustomerId { get; set; }
}