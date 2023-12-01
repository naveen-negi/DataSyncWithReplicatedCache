    using System.ComponentModel.DataAnnotations.Schema;
    using Sessions.API.Controllers;

    public enum SessionStatus
    {
        Started,
        Stopped,
        Failed
    } 
    
public class SessionEntity
{
    [Column(TypeName = "text")]
    public SessionStatus Status { get; private set; }
    
    public string UserId { get; private set;}
    
    public Guid Id { get; private set;}

    public SessionEntity(string locationId, string userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        LocationId = locationId;
        Status = SessionStatus.Started;
        StartDate = DateTime.UtcNow;
    }
    
    [Column(TypeName = "json")]
    public PaymentDetails? PaymentDetails { get; private set; }
    
    [Column(TypeName = "json")]
    public PricingCalculation? PricingCalculation { get; private set; }
    public DateTime StartDate { get;  private set;}
    public DateTime? EndDate { get;  private set;}
    public string LocationId { get;  private set;}

    public SessionEntity Stop()
    {
        Status = SessionStatus.Stopped;
        EndDate = DateTime.UtcNow;
        return this;
    }

    public SessionEntity UpdatePaymentDetails(PaymentDetails paymentDetails)
    {
        PaymentDetails = paymentDetails;
        return this;
    }

    public SessionEntity UpdatePricingDetails(PricingCalculation pricingCalculation)
    {
        PricingCalculation = pricingCalculation;
        return this;
    }
    public SessionEntity Rollback()
    {
        Status = SessionStatus.Started;
        EndDate = null;
        return this;
    }
    
    public override string ToString()
    {
        return $"Id: {Id}, UserId: {UserId}, LocationId: {LocationId}, StartDate: {StartDate}, EndDate: {EndDate}, Status: {Status}, PaymentDetails: {PaymentDetails}, PricingCalculation: {PricingCalculation}";
    }
}