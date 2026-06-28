using Domain.Capabilities;

namespace Domain.Pipelines;

/// <summary>
/// Contexto mutável que atravessa a Pipeline. Carrega o valor corrente entre steps,
/// o usuário, capabilities adicionais de runtime e a posição corrente.
/// </summary>
internal sealed class PipelineContext
{
    public PipelineId PipelineId { get; }
    public PipelineName PipelineName { get; }
    public object? CurrentValue { get; private set; }
    public Guid? UserId { get; }
    public DateTime StartedAt { get; }

    /// <summary>
    /// Capabilities extras injetadas no momento da execução (não são as do Agent).
    /// Permite à Pipeline fornecer plugins/MCPs específicos para uma run.
    /// </summary>
    public IReadOnlyList<PluginDescriptor> RuntimePlugins { get; }
    public IReadOnlyList<McpDescriptor> RuntimeMcps { get; }

    public StepName? CurrentStepName { get; private set; }
    public int CurrentStepIndex { get; private set; }
    public int TotalSteps { get; }

    public PipelineContext(
        PipelineId pipelineId,
        PipelineName pipelineName,
        int totalSteps,
        object? initialValue,
        Guid? userId = null,
        IReadOnlyList<PluginDescriptor>? runtimePlugins = null,
        IReadOnlyList<McpDescriptor>? runtimeMcps = null)
    {
        PipelineId = pipelineId;
        PipelineName = pipelineName;
        TotalSteps = totalSteps;
        CurrentValue = initialValue;
        UserId = userId;
        StartedAt = DateTime.UtcNow;
        RuntimePlugins = runtimePlugins ?? [];
        RuntimeMcps = runtimeMcps ?? [];
    }

    public void UpdateValue(object? value) => CurrentValue = value;

    public void MoveTo(int index, StepName stepName)
    {
        CurrentStepIndex = index;
        CurrentStepName = stepName;
    }

    public double ProgressPercent => TotalSteps == 0
        ? 0
        : (CurrentStepIndex + 1.0) / TotalSteps * 100;
}
