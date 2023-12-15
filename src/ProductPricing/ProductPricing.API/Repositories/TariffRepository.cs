using ProductPricing.API.Entities;

namespace ProductPricing.API.Repositories;

public interface ITariffRepository
{
    public Tariff? Get(DateTime start, DateTime end, string locationId);

    public Tariff Save(Tariff tariff);
}

public class TariffRepository : ITariffRepository
{
    private readonly TariffDBContext _tariffDbContext;

    public TariffRepository(TariffDBContext tariffDbContext)
    {
        _tariffDbContext = tariffDbContext;
    }

    public Tariff? Get(DateTime start, DateTime end, string locationId)
    {
        return _tariffDbContext.Tariffs.FirstOrDefault(t =>
            start >= t.ValidFrom && end <= t.ValidTo);
    }

    public Tariff Save(Tariff tariff)
    {
        _tariffDbContext.Tariffs.Add(tariff);
        return tariff;
    }
}