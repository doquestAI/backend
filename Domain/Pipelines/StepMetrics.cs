namespace Domain.Pipelines;

/// <summary>Métricas observáveis de um único <see cref="PipelineStep"/>.</summary>
public sealed class StepMetrics
{
    public TokenUsage Tokens { get; }
    public TimeSpan Duration { get; }
    public int LlmCallCount { get; }
    public DateTime StartedAt { get; }
    public DateTime FinishedAt { get; }

    public StepMetrics(
        TokenUsage tokens,
        TimeSpan duration,
        int llmCallCount,
        DateTime startedAt,
        DateTime finishedAt)
    {
        Tokens = tokens;
        Duration = duration;
        LlmCallCount = llmCallCount;
        StartedAt = startedAt;
        FinishedAt = finishedAt;
    }

    public static StepMetrics Empty(DateTime at) =>
        new(TokenUsage.Empty, TimeSpan.Zero, 0, at, at);
}
