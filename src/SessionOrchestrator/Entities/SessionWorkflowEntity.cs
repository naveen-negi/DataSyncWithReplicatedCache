using System.ComponentModel.DataAnnotations.Schema;
using Stateless;
using Stateless.Graph;

namespace SessionOrchestrator.Entities;

// This class stores the workflow state
// Also this class uses the stateless library to manage state transitions
// This class should also be able to hydrate the workflow state from the database
// It should expose methods to update the workflow state

public enum Trigger
{
    StartSession,
    PriceSession,
    PaySession,
    CompleteSession,
    FailSession,
    FinishedSession
}

public enum WorkflowState
{
    INIT,
    SESSIONS_STARTED,
    SESSION_PRICED,
    SESSION_PAID,
    SESSION_COMPLETED,
    SESSION_FAILED,
    SESSION_ENDED
}

public class SessionWorkflowEntity
{
    public Guid Id { get; set; }
    public string UserId { get; set; }

    [Column(TypeName = "text")]
    public WorkflowState WorkflowState { get; private set; }

    [NotMapped]
    private readonly StateMachine<WorkflowState, Trigger> _stateMachine;
    
    public SessionWorkflowEntity(Guid id, string userId, WorkflowState workflowState)
    {
        Id = id;
        UserId = userId;
        WorkflowState = workflowState;
        _stateMachine = new StateMachine<WorkflowState, Trigger>(() => WorkflowState, s => WorkflowState = s);
        ConfigureStateMachine();
    }

    private void ConfigureStateMachine()
    {
        _stateMachine.Configure(WorkflowState.SESSIONS_STARTED)
            .Permit(Trigger.FinishedSession, WorkflowState.SESSION_ENDED)
            .Permit(Trigger.FailSession, WorkflowState.SESSION_FAILED);

        _stateMachine.Configure(WorkflowState.SESSION_ENDED)
            .Permit(Trigger.PriceSession, WorkflowState.SESSION_PRICED)
            .Permit(Trigger.FailSession, WorkflowState.SESSION_FAILED);

        _stateMachine.Configure(WorkflowState.SESSION_PRICED)
            .Permit(Trigger.PaySession, WorkflowState.SESSION_PAID)
            .Permit(Trigger.FailSession, WorkflowState.SESSION_FAILED);

        _stateMachine.Configure(WorkflowState.SESSION_PAID)
            .Permit(Trigger.CompleteSession, WorkflowState.SESSION_COMPLETED)
            .Permit(Trigger.FailSession, WorkflowState.SESSION_FAILED);
    }
    
    public string ToDotGraph()
    {
        return UmlDotGraph.Format(_stateMachine.GetInfo());
    }

    public void StopSession()
    {
        // LATER: Perhaps we should also store when was session stopped. So, that we have one single place to look for.
        _stateMachine.Fire(Trigger.FinishedSession);
    }

    public void PriceSession()
    {
        _stateMachine.Fire(Trigger.PriceSession);
    }

    public void PaySession()
    {
        _stateMachine.Fire(Trigger.PaySession);
    }
}