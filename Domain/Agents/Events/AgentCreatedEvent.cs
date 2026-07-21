using Domain.Agents.ValueObjects;
using Domain.Shared.Core;

namespace Domain.Agents.Aggregates;

public sealed record AgentCreatedEvent(Guid AggregateId, string AgentName, AgentRole Role)
    : DomainEvent(AggregateId)
{
    public string EventType => "agent.created";
}

public sealed record AgentInvokedEvent(Guid AggregateId, long InputTokens, long OutputTokens, TimeSpan Duration)
    : DomainEvent(AggregateId)
{
    public string EventType => "agent.invoked";
}

public sealed record AgentDisabledEvent(Guid AggregateId, string AgentName)
    : DomainEvent(AggregateId)
{
    public string EventType => "agent.disabled";
}

public sealed record AgentEnabledEvent(Guid AggregateId, string AgentName)
    : DomainEvent(AggregateId)
{
    public string EventType => "agent.enabled";
}

public sealed record AgentSystemPromptUpdatedEvent(Guid AggregateId, string AgentName)
    : DomainEvent(AggregateId)
{
    public string EventType => "agent.system_prompt_updated";
}

public sealed record AgentCapabilitiesUpdatedEvent(Guid AggregateId, string AgentName)
    : DomainEvent(AggregateId)
{
    public string EventType => "agent.capabilities_updated";
}
