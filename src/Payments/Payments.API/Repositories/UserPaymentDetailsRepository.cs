using Payments.API.Entities;
using ProductPricing.API.Entities;

namespace Payments.API.Repositories;

public interface IUserPaymentDetailsRepository
{
    UserPaymentDetails? GetUser(string userId);
}

public class UserPaymentDetailsRepository : IUserPaymentDetailsRepository
{
    private readonly PaymentsDBContext _paymentsDbContext;
    
    public UserPaymentDetailsRepository(PaymentsDBContext paymentsDbContext)
    {
        _paymentsDbContext = paymentsDbContext;
    }
    
    public UserPaymentDetails? GetUser(string userId)
    {
       return _paymentsDbContext.UserPaymentDetails.FirstOrDefault(x => x.UserId == userId); 
    }
    
}