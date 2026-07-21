using Domain.Shared.Core;

namespace Domain.Sessions.Aggregates;

public sealed record SessionCreatedEvent(Guid AggregateId, Guid AgentId, Guid? UserId)
    : DomainEvent(AggregateId)
{
    public string EventType => "session.created";
}

public sealed record MemoryEntryAddedEvent(Guid AggregateId, Guid SessionId, object Role)
    : DomainEvent(AggregateId)
{
    public string EventType => "session.memory_entry_added";
}

public sealed record SessionPausedEvent(Guid AggregateId, Guid SessionId)
    : DomainEvent(AggregateId)
{
    public string EventType => "session.paused";
}

public sealed record SessionResumedEvent(Guid AggregateId, Guid SessionId)
    : DomainEvent(AggregateId)
{
    public string EventType => "session.resumed";
}

public sealed record SessionClosedEvent(Guid AggregateId, Guid SessionId)
    : DomainEvent(AggregateId)
{
    public string EventType => "session.closed";
}

public sealed record ExecutionFailedEvent(Guid AggregateId, Guid SessionId, string ActionName, string? ErrorMessage)
    : DomainEvent(AggregateId)
{
    public string EventType => "session.execution_failed";
}

public sealed record SessionMemoryClearedEvent(Guid AggregateId, Guid SessionId)
    : DomainEvent(AggregateId)
{
    public string EventType => "session.memory_cleared";
}
