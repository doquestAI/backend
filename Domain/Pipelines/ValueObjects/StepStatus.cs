namespace Domain.Pipelines.ValueObjects;

/// <summary>
/// Status de um passo individual da pipeline.
/// </summary>
public enum StepStatus
{
    /// <summary>Aguardando execução.</summary>
    Pending = 0,

    /// <summary>Executando.</summary>
    Running = 1,

    /// <summary>Completou com sucesso.</summary>
    Completed = 2,

    /// <summary>Falhou.</summary>
    Failed = 3,

    /// <summary>Pulado (condição não atendida).</summary>
    Skipped = 4,
}
