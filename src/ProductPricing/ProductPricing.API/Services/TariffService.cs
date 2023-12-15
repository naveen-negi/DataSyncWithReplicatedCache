using MediatR;
using Microsoft.Extensions.Options;
using ProductPricing.API.Dtos;
using ProductPricing.API.Entities;
using ProductPricing.API.EventHandlers.Events;
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
    private readonly IMediator _mediator;

    public TariffService(ITariffRepository tariffRepository,
        IMediator mediator)
    {
        _tariffRepository = tariffRepository;
        _mediator = mediator;
    }

    public async Task<SessionPrice> CalculatePrice(SessionPricingRequest session)
    {
        var tariff = _tariffRepository.Get(session.Start, session.End, session.LocationId);
        if (tariff == null)
        {
            throw new Exception(string.Format("No tariff found %s"));
        }

        var priceForSession = tariff.CalculatePrice(session);
        await _mediator.Send(new SessionPriced(session.SessionId, priceForSession.Price,
            priceForSession.PriceAfterTax, priceForSession.TaxBasisPoints, priceForSession.TaxAmount));

        return priceForSession;
    }
}