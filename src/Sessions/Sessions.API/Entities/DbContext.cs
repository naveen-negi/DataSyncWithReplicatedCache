using Microsoft.EntityFrameworkCore;

namespace Sessions.API.Entities;

// Reference: https://github.com/cornflourblue/dotnet-6-crud-api/blob/master/Entities/User.cs
public class SessionDBContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public SessionDBContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Configuration.GetConnectionString("SessionsDB"));
    }


    public DbSet<SessionEntity> Sessions { get; set; } = null!;
}