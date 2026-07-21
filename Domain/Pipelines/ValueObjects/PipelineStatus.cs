namespace Domain.Pipelines.ValueObjects;

/// <summary>
/// Status de execução da pipeline.
/// </summary>
public enum PipelineStatus
{
    /// <summary>Aguardando execução.</summary>
    Pending = 0,

    /// <summary>Executando.</summary>
    Running = 1,

    /// <summary>Completou com sucesso.</summary>
    Completed = 2,

    /// <summary>Falhou durante execução.</summary>
    Failed = 3,

    /// <summary>Cancelada pelo usuário.</summary>
    Cancelled = 4,
}
