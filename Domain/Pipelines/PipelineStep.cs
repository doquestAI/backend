using Domain.Observability;
using System.Diagnostics;

namespace Domain.Pipelines;

/// <summary>
/// Step de uma Pipeline. Entidade abstrata — herde para criar steps específicos
/// (ex: <c>AgentStep</c>, <c>ValidationStep</c>, <c>LoggingStep</c>).
///
/// Cada execução emite um span filho <c>Pipeline.Step/{Name}</c> dentro do span
/// pai da Pipeline, via <see cref="PipelineTelemetry.Source"/>.
/// </summary>
public abstract class PipelineStep
{
    public StepName Name { get; }
    public StepStatus Status { get; protected set; } = StepStatus.Pending;

    protected PipelineStep(StepName name) => Name = name;
    protected PipelineStep(string name) : this(new StepName(name)) { }

    /// <summary>
    /// Executa este step. Emite span OTel, mede duração e atualiza métricas.
    /// Em caso de exceção, registra o evento no span e retorna falha estruturada.
    /// </summary>
    public async Task<StepResult> ExecuteAsync(PipelineContext context, CancellationToken cancellationToken)
    {
        Status = StepStatus.Running;
        var startedAt = DateTime.UtcNow;
        var sw = Stopwatch.StartNew();

        using var activity = PipelineTelemetry.Source.StartActivity(
            $"Pipeline.Step/{Name.Value}", ActivityKind.Internal);

        activity?.SetTag(TelemetryConstants.Tags.StepName, Name.Value);
        activity?.SetTag(TelemetryConstants.Tags.StepIndex, context.CurrentStepIndex);
        activity?.SetTag(TelemetryConstants.Tags.PipelineName, context.PipelineName.Value);

        var stepTags = new TagList
        {
            { TelemetryConstants.Tags.StepName, Name.Value },
            { TelemetryConstants.Tags.PipelineName, context.PipelineName.Value }
        };

        PipelineTelemetry.StepExecutions.Add(1, stepTags);

        try
        {
            var result = await OnExecuteAsync(context, cancellationToken);
            sw.Stop();
            Status = result.IsValid ? StepStatus.Succeeded : StepStatus.Failed;

            PipelineTelemetry.StepDuration.Record(sw.Elapsed.TotalMilliseconds, stepTags);

            if (result.IsValid)
            {
                activity?.SetStatus(ActivityStatusCode.Ok);
            }
            else
            {
                var firstMessage = result.Notifications.FirstOrDefault()?.Message;
                activity?.SetStatus(ActivityStatusCode.Error, firstMessage);
                PipelineTelemetry.StepFailures.Add(1, stepTags);
            }

            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            Status = StepStatus.Failed;

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddEvent(new ActivityEvent("exception", tags: new ActivityTagsCollection
            {
                ["exception.type"]       = ex.GetType().FullName,
                ["exception.message"]    = ex.Message,
                ["exception.stacktrace"] = ex.ToString()
            }));

            PipelineTelemetry.StepFailures.Add(1, stepTags);
            PipelineTelemetry.StepDuration.Record(sw.Elapsed.TotalMilliseconds, stepTags);

            var metrics = new StepMetrics(TokenUsage.Empty, sw.Elapsed, 0, startedAt, DateTime.UtcNow);
            return StepResult.Fail(Name.Value, $"Step threw: {ex.Message}", metrics);
        }
    }

    /// <summary>Implementação do step. Não chamar diretamente — use <see cref="ExecuteAsync"/>.</summary>
    protected abstract Task<StepResult> OnExecuteAsync(PipelineContext context, CancellationToken cancellationToken);
}
