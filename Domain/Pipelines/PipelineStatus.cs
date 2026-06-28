namespace Domain.Pipelines;

/// <summary>Ciclo de vida da Pipeline.</summary>
public enum PipelineStatus
{
    Pending = 0,
    Running = 1,
    Completed = 2,
    Failed = 3,
}
