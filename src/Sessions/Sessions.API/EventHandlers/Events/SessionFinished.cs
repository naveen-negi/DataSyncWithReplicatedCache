using MediatR;

namespace Sessions.API.EventHandlers.Events;

public class SessionFinished : IRequest
{
    public string UserId { get; }
    public string LocationId { get; }
    public DateTime Start { get; }
    public DateTime? End { get; }
    public string SessionId { get; }
    public SessionStatus Status { get; }


    public SessionFinished(string sessionId, string userId, string LocationId, DateTime start, DateTime? end)
    {
        UserId = userId;
        this.LocationId = LocationId;
        Start = start;
        End = end;
        SessionId = sessionId;
        Status = SessionStatus.Stopped;
    }
}