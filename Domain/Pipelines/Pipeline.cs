using Domain.Capabilities;
using Domain.Interfaces.Agents;
using Domain.Interfaces.Pipelines;
using Domain.Observability;
using Domain.Pipelines.Steps;
using Flunt.Notifications;
using System.Diagnostics;

namespace Domain.Pipelines;

/// <summary>
/// Pipeline tipada — entidade de negócio que orquestra uma lista ordenada de
/// <see cref="PipelineStep"/> e produz um <see cref="PipelineResult{TOut}"/>.
///
/// 100% POO: cada step é uma classe; cada pipeline é uma classe que herda
/// <see cref="Pipeline{TIn,TOut}"/> e configura seus steps no construtor.
///
/// Pode hospedar múltiplos agents (cada um em seu <c>AgentStep</c>),
/// agregando tokens e latência em <see cref="Metrics"/>.
///
/// Emite spans OTel via <see cref="PipelineTelemetry.Source"/> e métricas via
/// <see cref="PipelineTelemetry"/>. Não há dependência de NuGet — usa apenas BCL.
/// </summary>
public class Pipeline<TIn, TOut> : IPipeline<TIn, TOut>
{
    public PipelineId Id { get; }
    public PipelineName Name { get; protected set; }
    public PipelineStatus Status { get; private set; } = PipelineStatus.Pending;
    public PipelineMetrics Metrics { get; } = new();
    public PipelineExecution? CurrentExecution { get; private set; }

    private readonly List<PipelineStep> _steps = [];
    public IReadOnlyList<PipelineStep> Steps => _steps;

    public Pipeline(string name)
    {
        Id = PipelineId.New();
        Name = new PipelineName(name);
    }

    /// <summary>Adiciona um step arbitrário (composição fluente).</summary>
    public Pipeline<TIn, TOut> AddStep(PipelineStep step)
    {
        _steps.Add(step);
        return this;
    }

    /// <summary>Conveniência: adiciona um <see cref="ValidationStep"/>.</summary>
    public Pipeline<TIn, TOut> Validate(Func<object?, bool> predicate, string property, string message) =>
        AddStep(new ValidationStep(property, predicate, message));

    /// <summary>
    /// Executa a Pipeline com o input. Acumula tokens/tempo em <see cref="Metrics"/>.
    /// Em caso de falha de um step, encerra (short-circuit) e retorna notificações.
    /// Emite um span pai <c>Pipeline.Run/{Name}</c> com spans filhos por step.
    /// </summary>
    public async Task<PipelineResult<TOut>> RunAsync(
        TIn input,
        CancellationToken cancellationToken = default,
        Guid? userId = null,
        IReadOnlyList<PluginDescriptor>? runtimePlugins = null,
        IReadOnlyList<McpDescriptor>? runtimeMcps = null)
    {
        Status = PipelineStatus.Running;
        CurrentExecution = new PipelineExecution(Id, _steps.Count);

        var context = new PipelineContext(
            Id, Name, _steps.Count, input, userId, runtimePlugins, runtimeMcps);

        // ── Span pai de toda a execução da pipeline ──────────────────────
        using var activity = PipelineTelemetry.Source.StartActivity(
            $"Pipeline.Run/{Name.Value}", ActivityKind.Internal);

        activity?.SetTag(TelemetryConstants.Tags.PipelineName, Name.Value);
        activity?.SetTag(TelemetryConstants.Tags.PipelineId, Id.Value.ToString());
        activity?.SetTag(TelemetryConstants.Tags.PipelineStepCount, _steps.Count);
        if (userId.HasValue)
            activity?.SetTag(TelemetryConstants.Tags.UserId, userId.Value.ToString());

        var metricTags = new TagList
        {
            { TelemetryConstants.Tags.PipelineName, Name.Value }
        };

        PipelineTelemetry.PipelineExecutions.Add(1, metricTags);
        var sw = Stopwatch.StartNew();

        for (var i = 0; i < _steps.Count; i++)
        {
            var step = _steps[i];
            context.MoveTo(i, step.Name);
            CurrentExecution.MoveTo(i, step.Name);

            var stepResult = await step.ExecuteAsync(context, cancellationToken);
            Metrics.Accumulate(stepResult.Metrics);

            if (step is AgentStep agentStep)
                Metrics.AttributeTo(agentStep.AgentName, stepResult.Metrics.Tokens);

            if (!stepResult.IsValid)
            {
                Status = PipelineStatus.Failed;
                CurrentExecution.Fail();

                sw.Stop();
                var failureMessage = stepResult.Notifications.FirstOrDefault()?.Message;
                activity?.SetStatus(ActivityStatusCode.Error, failureMessage);
                activity?.SetTag(TelemetryConstants.Tags.PipelineStatus, "failed");

                PipelineTelemetry.PipelineFailures.Add(1, metricTags);
                PipelineTelemetry.PipelineDuration.Record(sw.Elapsed.TotalMilliseconds, metricTags);

                return PipelineResult<TOut>.Fail(
                    stepResult.Notifications.ToArray(), Metrics, Id);
            }

            context.UpdateValue(stepResult.Value);
        }

        Status = PipelineStatus.Completed;
        CurrentExecution.Complete();

        sw.Stop();
        activity?.SetStatus(ActivityStatusCode.Ok);
        activity?.SetTag(TelemetryConstants.Tags.PipelineStatus, "completed");
        activity?.SetTag(TelemetryConstants.Tags.LlmTokensInput, Metrics.TotalTokens.InputTokens);
        activity?.SetTag(TelemetryConstants.Tags.LlmTokensOutput, Metrics.TotalTokens.OutputTokens);
        PipelineTelemetry.PipelineDuration.Record(sw.Elapsed.TotalMilliseconds, metricTags);

        if (context.CurrentValue is TOut typed)
            return PipelineResult<TOut>.Success(typed, Metrics, Id);

        if (context.CurrentValue is null && default(TOut) is null)
            return PipelineResult<TOut>.Success(default!, Metrics, Id);

        var note = new Notification(
            nameof(Pipeline<TIn, TOut>),
            $"Pipeline output type mismatch — expected {typeof(TOut).Name}, got {context.CurrentValue?.GetType().Name ?? "null"}.");

        activity?.SetStatus(ActivityStatusCode.Error, note.Message);
        activity?.SetTag(TelemetryConstants.Tags.PipelineStatus, "failed");
        PipelineTelemetry.PipelineFailures.Add(1, metricTags);

        return PipelineResult<TOut>.Fail([note], Metrics, Id);
    }

    Task<PipelineResult<TOut>> IPipeline<TIn, TOut>.RunAsync(TIn input, CancellationToken ct)
        => RunAsync(input, ct);
}
