using Domain.Pipelines.ValueObjects;
using Domain.Shared.Core;

namespace Domain.Pipelines.Aggregates;

public sealed record PipelineCreatedEvent(Guid AggregateId, string PipelineName)
    : DomainEvent(AggregateId)
{
    public string EventType => "pipeline.created";
}

public sealed record PipelineStartedEvent(Guid AggregateId, Guid PipelineId, int StepCount)
    : DomainEvent(AggregateId)
{
    public string EventType => "pipeline.started";
}

public sealed record StepCompletedEvent(Guid AggregateId, Guid PipelineId, string StepName, TokenMetrics Tokens)
    : DomainEvent(AggregateId)
{
    public string EventType => "pipeline.step_completed";
}

public sealed record PipelineCompletedEvent(Guid AggregateId, Guid PipelineId, int StepsExecuted, TokenMetrics TotalTokens)
    : DomainEvent(AggregateId)
{
    public string EventType => "pipeline.completed";
}

public sealed record PipelineFailedEvent(Guid AggregateId, Guid PipelineId, string FailedStepName, string ErrorMessage)
    : DomainEvent(AggregateId)
{
    public string EventType => "pipeline.failed";
}

public sealed record PipelineCancelledEvent(Guid AggregateId, Guid PipelineId)
    : DomainEvent(AggregateId)
{
    public string EventType => "pipeline.cancelled";
}
