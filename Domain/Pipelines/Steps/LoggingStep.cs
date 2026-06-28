using Microsoft.Extensions.Logging;

namespace Domain.Pipelines.Steps;

/// <summary>Step que registra o valor corrente via <see cref="ILogger"/>. Pass-through.</summary>
public sealed class LoggingStep : PipelineStep
{
    private readonly ILogger _logger;
    private readonly Func<PipelineContext, string> _messageBuilder;
    private readonly LogLevel _level;

    public LoggingStep(
        string name,
        ILogger logger,
        Func<PipelineContext, string> messageBuilder,
        LogLevel level = LogLevel.Information)
        : base(name)
    {
        _logger = logger;
        _messageBuilder = messageBuilder;
        _level = level;
    }

    protected override Task<StepResult> OnExecuteAsync(
        PipelineContext context, CancellationToken cancellationToken)
    {
        var startedAt = DateTime.UtcNow;
        _logger.Log(_level, "{Message}", _messageBuilder(context));
        var metrics = StepMetrics.Empty(startedAt);
        return Task.FromResult(StepResult.Success(context.CurrentValue, metrics));
    }
}
