using Microsoft.AspNetCore.Mvc;
using SessionOrchestrator.Workflows;

namespace SessionOrchestrator.Controllers;

[ApiController]
[Route("[controller]")]
public class SessionController : ControllerBase
{
    private readonly ILogger<SessionController> _logger;

    private readonly SagaPattern _sagaPattern = new();

    public SessionController(ILogger<SessionController> logger)
    {
        _logger = logger;
    }

    // What kind of entity should a distributed transaction return
    [HttpPost(Name = "EndSession")]
    public SessionTransactionResult End(SessionEndRequest request)
    {
        return _sagaPattern.SessionTransactionResult(request);
    }

}