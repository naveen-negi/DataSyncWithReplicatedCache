using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Payments.API.Repositories;

public interface ISessionOrchestratorApi
{
    [Post("/api/workflow/{sessionId}/payment")]
    Task UpdatePaymentDetails(string sessionId, [Body] PaymentDetailsRequest request);
}

public record PaymentDetailsRequest(string SessionId, string Status);