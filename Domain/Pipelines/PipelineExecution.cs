namespace Domain.Pipelines;

/// <summary>
/// Representa UMA execução concreta de uma Pipeline. Vive enquanto a Pipeline roda,
/// rastreia o step corrente e os tempos. Snapshot para observabilidade externa.
/// </summary>
public sealed class PipelineExecution
{
    public PipelineId PipelineId { get; }
    public DateTime StartedAt { get; }
    public DateTime? FinishedAt { get; private set; }
    public StepName? CurrentStepName { get; private set; }
    public int CurrentStepIndex { get; private set; }
    public int TotalSteps { get; }
    public PipelineStatus Status { get; private set; }

    public PipelineExecution(PipelineId pipelineId, int totalSteps)
    {
        PipelineId = pipelineId;
        TotalSteps = totalSteps;
        StartedAt = DateTime.UtcNow;
        Status = PipelineStatus.Running;
    }

    public void MoveTo(int index, StepName stepName)
    {
        CurrentStepIndex = index;
        CurrentStepName = stepName;
    }

    public void Complete()
    {
        Status = PipelineStatus.Completed;
        FinishedAt = DateTime.UtcNow;
    }

    public void Fail()
    {
        Status = PipelineStatus.Failed;
        FinishedAt = DateTime.UtcNow;
    }

    public TimeSpan Elapsed => (FinishedAt ?? DateTime.UtcNow) - StartedAt;

    public double ProgressPercent => TotalSteps == 0
        ? 0
        : (CurrentStepIndex + 1.0) / TotalSteps * 100;
}
