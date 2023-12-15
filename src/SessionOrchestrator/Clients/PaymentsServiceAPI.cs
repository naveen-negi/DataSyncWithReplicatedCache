using Microsoft.AspNetCore.Mvc;
using Refit;
using SessionOrchestrator.Controllers.Dto;
using SessionOrchestrator.Transactions;
using static SessionOrchestrator.Transactions.Transaction;

namespace SessionOrchestrator.Clients;

public record BilledSessionRequest(string SessionId, string UserId,
    decimal PriceAfterTax, decimal TaxAmount,
    int TaxBasisPoints, string Currency = "EUR");

[Saga(new[] { SESSION_STARTED, SESSION_STOPPED })]
public interface IPaymentsServiceApi
{
    [Post("/api/payments/pay")]
    public Task ProcessPayment([Body] BilledSessionRequest request);
}