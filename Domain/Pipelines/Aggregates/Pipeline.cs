using Domain.Pipelines.ValueObjects;
using Domain.Shared.Core;

namespace Domain.Pipelines.Aggregates;

/// <summary>
/// AGGREGATE ROOT: Pipeline
/// Orquestra execução de múltiplos steps em sequência.
/// Encapsula lógica de retry, short-circuit em erro, métricas acumuladas.
/// </summary>
public sealed class Pipeline : AggregateRoot
{
    private readonly List<PipelineStep> _steps = [];

    public PipelineId PipelineId { get; private set; } = null!;
    public PipelineName Name { get; private set; } = null!;
    public PipelineStatus Status { get; private set; } = PipelineStatus.Pending;
    public IReadOnlyList<PipelineStep> Steps => _steps.AsReadOnly();
    public int CompletedStepCount { get; private set; }
    public TokenMetrics TotalTokens { get; private set; } = TokenMetrics.Empty;
    public TimeSpan TotalDuration { get; private set; }

    private Pipeline() { }

    public static Pipeline Create(PipelineName name)
    {
        var pipeline = new Pipeline
        {
            Id = Guid.NewGuid(),
            PipelineId = PipelineId.New(),
            Name = name,
            Status = PipelineStatus.Pending,
        };

        pipeline.RaiseDomainEvent(new PipelineCreatedEvent(pipeline.Id, name.Value));
        return pipeline;
    }

    public void AddStep(PipelineStep step)
    {
        if (Status != PipelineStatus.Pending)
        {
            AddNotification(nameof(Steps), "Cannot add steps to pipeline that is not pending");
            return;
        }

        _steps.Add(step);
    }

    public void Start()
    {
        if (Status != PipelineStatus.Pending)
        {
            AddNotification(nameof(Status), "Only pending pipelines can start");
            return;
        }

        Status = PipelineStatus.Running;
        RaiseDomainEvent(new PipelineStartedEvent(Id, PipelineId.Value, _steps.Count));
    }

    public void CompleteStep(int stepIndex, TokenMetrics tokens, TimeSpan duration)
    {
        if (stepIndex < 0 || stepIndex >= _steps.Count)
        {
            AddNotification(nameof(stepIndex), "Invalid step index");
            return;
        }

        var step = _steps[stepIndex];
        step.Complete(tokens, duration);
        CompletedStepCount++;
        TotalTokens = TotalTokens.Add(tokens);
        TotalDuration += duration;

        RaiseDomainEvent(new StepCompletedEvent(Id, PipelineId.Value, step.Name.Value, tokens));
    }

    public void FailStep(int stepIndex, string errorMessage)
    {
        if (stepIndex < 0 || stepIndex >= _steps.Count)
        {
            AddNotification(nameof(stepIndex), "Invalid step index");
            return;
        }

        var step = _steps[stepIndex];
        step.Fail(errorMessage);
        Status = PipelineStatus.Failed;

        RaiseDomainEvent(new PipelineFailedEvent(Id, PipelineId.Value, step.Name.Value, errorMessage));
    }

    public void Complete()
    {
        if (Status != PipelineStatus.Running)
        {
            AddNotification(nameof(Status), "Only running pipelines can complete");
            return;
        }

        Status = PipelineStatus.Completed;
        RaiseDomainEvent(new PipelineCompletedEvent(Id, PipelineId.Value, CompletedStepCount, TotalTokens));
    }

    public void Cancel()
    {
        if (Status == PipelineStatus.Completed || Status == PipelineStatus.Failed)
        {
            AddNotification(nameof(Status), "Cannot cancel finished pipeline");
            return;
        }

        Status = PipelineStatus.Cancelled;
        RaiseDomainEvent(new PipelineCancelledEvent(Id, PipelineId.Value));
    }
}

/// <summary>
/// Entidade: Passo da pipeline.
/// Não é aggregate root, faz parte do agregado Pipeline.
/// </summary>
public sealed class PipelineStep : Entity
{
    public StepName Name { get; }
    public StepStatus Status { get; private set; } = StepStatus.Pending;
    public TokenMetrics Tokens { get; private set; } = TokenMetrics.Empty;
    public TimeSpan Duration { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int Order { get; }

    public PipelineStep(StepName name, int order = 0)
    {
        Name = name;
        Order = order;
    }

    public void Complete(TokenMetrics tokens, TimeSpan duration)
    {
        Status = StepStatus.Completed;
        Tokens = tokens;
        Duration = duration;
    }

    public void Fail(string errorMessage)
    {
        Status = StepStatus.Failed;
        ErrorMessage = errorMessage;
    }

    public void Skip()
    {
        Status = StepStatus.Skipped;
    }
}
