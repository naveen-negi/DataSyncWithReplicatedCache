namespace SessionOrchestrator.Controllers.Dto;

public record SessionEndRequest(string SessionId);

public record SessionBillingUpdateRequest();

public record SessionPaymentUpdateRequest();

// status could be "started" or "stopped" or "failed"
public record SessionUpdateRequest(string SessionId, string Status,
    string UserId, string LocationId, DateTime Start, DateTime End);