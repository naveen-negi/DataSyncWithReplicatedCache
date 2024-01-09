using Microsoft.AspNetCore.Mvc;
using Users.API.Entities;
using Users.API.Services;

namespace Users.API.Controllers;

[Route("api")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly ICacheService _cacheService;

    public UsersController(ILogger<UsersController> logger, ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    [HttpGet("users/{userId}")]
    public async Task<string> GetCustomer(string userId)
    {
        try
        {
            return _cacheService.Get(userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting customer");
            throw e;
        }
    }
    
    
    [HttpPost("users/{userId}")]
    public async Task<AcceptedResult> CreateCustomer(User request)
    {
        try
        {
            _cacheService.Add(request);
            return Accepted();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting customer");
            throw e;
        }
    }
}