using Refit;

namespace ProductPricing.API.Repositories;

public record TransactionResult();

//TODO: Should we pass a list of transactions or just one?
// list would again lead to batch over network, which is not ideal.
// for now we just pass one transaction
public record TransactionRequest(string SessionId, string UserId, decimal PriceAfterTax, decimal TaxAmount, int TaxBasisPoints, string Currency = "EUR");

public interface ITransactionServiceApi
{
    [Post("/api/payments/pay")]
    public Task<TransactionResult> Transact([Body] TransactionRequest transactions);
}