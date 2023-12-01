using Payments.API.Controllers;
using Payments.API.Entities;
using ProductPricing.API.Entities;

namespace Payments.API.Repositories;

public interface IPaymentRepository
{
    void Save(Payment payment);
}

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentsDBContext _paymentsDbContext;

    public PaymentRepository(PaymentsDBContext paymentsDbContext)
    {
        _paymentsDbContext = paymentsDbContext;
    }
        
    public void Save(Payment payment)
    {
        _paymentsDbContext.Payments.Add(payment);
        _paymentsDbContext.SaveChanges();
    }
}