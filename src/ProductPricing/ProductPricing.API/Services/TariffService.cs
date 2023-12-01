using Microsoft.Extensions.Options;
using ProductPricing.API.Dtos;
using ProductPricing.API.Entities;
using ProductPricing.API.Repositories;
using Refit;

namespace ProductPricing.API.Services;

public interface ITariffService
{
    Task<SessionPrice> CalculatePrice(SessionPricingRequest session);
}

public class TariffService : ITariffService
{
    private readonly ITariffRepository _tariffRepository;
    private readonly ITransactionServiceApi _transactionServiceApi;

    public TariffService(ITariffRepository tariffRepository, IOptions<PaymentsServiceConfig> config)
    {
        _tariffRepository = tariffRepository;
        _transactionServiceApi = RestService.For<ITransactionServiceApi>(config.Value.BaseUrl);
    }
    
    public async Task<SessionPrice> CalculatePrice(SessionPricingRequest session)
    {
            // How would you handle duplicate requests? 
            // since we are not saving anything in the database, we can't deduplicate
            // If session service has retry logic, we might get duplicate requests. 
            // So, we should have some kind of deduplication logic here
            // Alternatively, we can have some kind of idempotency key in the request
            // and hope that Transaction Service is idempotent
        var tariff = _tariffRepository.Get(session.Start, session.End, session.LocationId);
        if(tariff == null)
        {
            throw new Exception(string.Format("No tariff found %s"));
        }
        var priceForSession = tariff.CalculatePrice(session);
        
        try
        {
            var transaction = new TransactionRequest(session.SessionId, session.UserId, priceForSession.PriceAfterTax,
                priceForSession.TaxAmount, priceForSession.TaxBasisPoints, "EUR");
            await _transactionServiceApi.Transact(transaction);
        }
        catch (Exception e)
        {
            // TODO: Since call to transaction service failed, we need to send error to session service
            throw new Exception(e.Message);
        }

        return priceForSession;
    }
}