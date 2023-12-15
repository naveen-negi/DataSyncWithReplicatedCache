namespace SessionOrchestrator.Transactions;

public enum Transaction
{
    // These enums are based on per saga (one enum for one saga)
    SESSION_STARTED,
    SESSION_STOPPED,
}

[AttributeUsage(AttributeTargets.Interface)]
public class SagaAttribute : Attribute
{
    public Transaction[] Transaction { get; }

    public SagaAttribute(Transaction[] transaction)
    {
        Transaction = transaction;
    }

    public SagaAttribute(Transaction transaction)
    {
        Transaction = new[] { transaction };
    }
}

public enum Communication
{
    SYNC,
    ASYNC,
}

public enum Consistency
{
    Atomic,
    Eventual,
}

public enum Coordination
{
    Orchestration,
    Choreography,
}

[AttributeUsage(AttributeTargets.Class)]
public class CommunicationTypeAttribute : Attribute
{
    public Communication Communication { get; }

    public CommunicationTypeAttribute(Communication communication)
    {
        Communication = communication;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ConsistencyTypeAttribute : Attribute
{
    public Consistency Consistency { get; }

    public ConsistencyTypeAttribute(Consistency consistency)
    {
        Consistency = consistency;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class CoordinationTypeAttribute : Attribute
{
    public Coordination Coordination { get; }

    public CoordinationTypeAttribute(Coordination coordination)
    {
        Coordination = coordination;
    }
}