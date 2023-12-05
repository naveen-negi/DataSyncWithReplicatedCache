using SessionOrchestrator.Services;

namespace SessionOrchestrator.Controllers;

public record SessionTransactionResult(bool IsSuccess, PaymentResult? PaymentDetails);

public record SessionEndRequest();
