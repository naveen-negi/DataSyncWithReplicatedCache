using Microsoft.AspNetCore.Mvc;
using ProductPricing.API.Dtos;
using ProductPricing.API.Entities;
using ProductPricing.API.Services;

namespace ProductPricing.API.Controllers;

[Route("api")]
public class PricingController : ControllerBase
{
    private readonly ILogger<PricingController> _logger;
    private readonly ITariffService _tariffService;
    private readonly ICacheService _cacheService;

    public PricingController(ILogger<PricingController> logger, ITariffService tariffService,
        ICacheService cacheService)
    {
        _logger = logger;
        _tariffService = tariffService;
        _cacheService = cacheService;
    }

    [HttpPost("prices/calculate")]
    public async Task<IActionResult> CalculatePrices([FromBody] SessionPricingRequest session)
    {
        _logger.LogInformation("request received: {Session}", session.ToString());
        try
        {
            await _tariffService.CalculatePrice(session);
            return Accepted();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            // you can have some modes in each service which you can set to simulate failure
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while calculating price for session" });
        }
    }
    
    
    [HttpGet("prices/try")]
    public async Task<User> GetCacheEntry()
    {
        try
        {
            _logger.LogInformation("request received for cache entry");
            return _cacheService.Get("John");
        }
        catch (Exception e)
        {
            // _logger.LogError(e.Message);
            // // you can have some modes in each service which you can set to simulate failure
            // return StatusCode(StatusCodes.Status500InternalServerError,
            //     new { message = "An error occurred while calculating price for session" });
            throw;
        }
    }
}