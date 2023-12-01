using Microsoft.EntityFrameworkCore;
using Payments.API.Entities;

namespace ProductPricing.API.Entities;

public class PaymentsDBContext : DbContext
{
    private readonly IConfiguration Configuration;
        
        public PaymentsDBContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("PaymentsDB"));
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserPaymentDetails>().HasData(new UserPaymentDetails(){ Id = "1", UserId = "1", StripeCustomerId = "cus_123" }, 
            new { Id = "2", UserId = "9999", StripeCustomerId = "cus_999" });
        }
            
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<UserPaymentDetails> UserPaymentDetails { get; set; } = null!;
}