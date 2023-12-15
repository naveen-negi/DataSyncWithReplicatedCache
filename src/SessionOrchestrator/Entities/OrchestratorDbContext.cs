using Microsoft.EntityFrameworkCore;
using SessionOrchestrator.Entities;

namespace Sessions.API.Entities;

public class OrchestratorDBContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public OrchestratorDBContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Configuration.GetConnectionString("SessionOrchestratorDB"));
    }


    public DbSet<SessionWorkflowEntity?> SessionWorkflows { get; set; } = null!;
}