using MediatR;

namespace Payments.API.EventHandlers.Events;

public class SessionPaid : IRequest
{
    public string SessionId { get; }
    
    public string Status { get; } = "Paid";
    
    public SessionPaid( string sessionId)
    {
        SessionId = sessionId;
    }
}