using Microsoft.EntityFrameworkCore;

namespace ProductPricing.API.Entities;

public class TariffDBContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public TariffDBContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Configuration.GetConnectionString("TariffDB"));
    }

    // Let's now seed some initial data
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var from = DateTime.Now.Subtract(TimeSpan.FromDays(365)).ToUniversalTime();
        var to = DateTime.Now.Add(TimeSpan.FromDays(365)).ToUniversalTime();
        modelBuilder.Entity<Tariff>().HasData(new Tariff(from, to, 2, "1234", 1900));
    }


    public DbSet<Tariff> Tariffs { get; set; } = null!;
}