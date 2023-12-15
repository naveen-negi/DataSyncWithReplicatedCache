namespace Payments.API.Entities;

public enum PaymentStatus
{
    Pending,
    Paid,
    Failed
}

public record Payment()
{
    public DateTime Created { get; set; }

    public Guid PaymentId { get; set; }

    public string StripeCustomerId { get; set; }
    public string UserId { get; set; }
    public string SessionId { get; set; }

    public PaymentStatus Status { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }

    public string CreatedAt { get; private set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
    public string UpdatedAt { get; private set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
    public string TransactionId { get; set; }
}