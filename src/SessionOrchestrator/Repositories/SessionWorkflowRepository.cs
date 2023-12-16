using Microsoft.EntityFrameworkCore;
using SessionOrchestrator.Entities;
using SessionOrchestrator.Workflows;
using Sessions.API.Entities;

namespace SessionOrchestrator.Repositories;

public interface ISessionWorkflowRepository
{
    Task<SessionWorkflowEntity?> GetSessionWorkflow(Guid sessionId);
    Task<SessionWorkflowEntity> SaveSessionWorkflow(SessionWorkflowEntity sessionWorkflow);
}

public class SessionWorkflowRepository : ISessionWorkflowRepository
{
    private readonly OrchestratorDBContext _dbContext;
    private readonly ILogger<SessionWorkflowRepository> _logger;

    public SessionWorkflowRepository(OrchestratorDBContext dbContext, ILogger<SessionWorkflowRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<SessionWorkflowEntity?> GetSessionWorkflow(Guid sessionId)
    {
        return await _dbContext.SessionWorkflows.FirstOrDefaultAsync(x => x != null && x.Id == sessionId);
    }

    
    public async Task<SessionWorkflowEntity> SaveSessionWorkflow(SessionWorkflowEntity sessionWorkflow)
    {
        _logger.LogInformation("Saving session workflow {State}", sessionWorkflow.WorkflowState);
        // Check if the entity already exists in the database
        var existingEntity = await _dbContext.SessionWorkflows.FindAsync(sessionWorkflow.Id);

        if (existingEntity == null)
        {
            await _dbContext.SessionWorkflows.AddAsync(sessionWorkflow);
        }
        else
        {
            _dbContext.Entry(existingEntity).CurrentValues.SetValues(sessionWorkflow);
        }

        // Save changes to the database
        await _dbContext.SaveChangesAsync();
        return sessionWorkflow;
    }
}