namespace Domain.Pipelines.Steps;

/// <summary>
/// Step que valida o valor corrente da pipeline. Falha = notificação,
/// curto-circuita os steps seguintes. Não consome tokens.
/// </summary>
public sealed class ValidationStep : PipelineStep
{
    private readonly Func<object?, bool> _predicate;
    private readonly string _property;
    private readonly string _message;

    public ValidationStep(string property, Func<object?, bool> predicate, string message)
        : base($"Validate:{property}")
    {
        _property = property;
        _predicate = predicate;
        _message = message;
    }

    protected override Task<StepResult> OnExecuteAsync(
        PipelineContext context, CancellationToken cancellationToken)
    {
        var startedAt = DateTime.UtcNow;
        var ok = _predicate(context.CurrentValue);
        var metrics = StepMetrics.Empty(startedAt);

        return Task.FromResult(ok
            ? StepResult.Success(context.CurrentValue, metrics)
            : StepResult.Fail(_property, _message, metrics));
    }
}

/// <summary>Versão tipada — recebe e valida o valor já convertido para <typeparamref name="T"/>.</summary>
public sealed class ValidationStep<T> : PipelineStep
{
    private readonly Func<T, bool> _predicate;
    private readonly string _property;
    private readonly string _message;

    public ValidationStep(string property, Func<T, bool> predicate, string message)
        : base($"Validate:{property}")
    {
        _property = property;
        _predicate = predicate;
        _message = message;
    }

    protected override Task<StepResult> OnExecuteAsync(
        PipelineContext context, CancellationToken cancellationToken)
    {
        var startedAt = DateTime.UtcNow;
        var metrics = StepMetrics.Empty(startedAt);

        if (context.CurrentValue is not T typed)
        {
            return Task.FromResult(StepResult.Fail(
                _property,
                $"Expected {typeof(T).Name}, got {context.CurrentValue?.GetType().Name ?? "null"}",
                metrics));
        }

        return Task.FromResult(_predicate(typed)
            ? StepResult.Success(typed, metrics)
            : StepResult.Fail(_property, _message, metrics));
    }
}
