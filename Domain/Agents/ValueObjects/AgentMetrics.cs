namespace Domain.Agents.ValueObjects;

/// <summary>
/// Métricas de desempenho de um agente.
/// Tokens consumidos, latência, número de invocações.
/// </summary>
public sealed class AgentMetrics
{
    public long TotalInputTokens { get; private set; }
    public long TotalOutputTokens { get; private set; }
    public long InvocationCount { get; private set; }
    public long FailureCount { get; private set; }
    public TimeSpan TotalDuration { get; private set; }

    public double AverageDurationMs =>
        InvocationCount > 0 ? TotalDuration.TotalMilliseconds / InvocationCount : 0;

    public long TotalTokens => TotalInputTokens + TotalOutputTokens;

    public void RecordInvocation(long inputTokens, long outputTokens, TimeSpan duration)
    {
        TotalInputTokens += inputTokens;
        TotalOutputTokens += outputTokens;
        TotalDuration += duration;
        InvocationCount++;
    }

    public void RecordFailure()
    {
        FailureCount++;
    }

    public void Reset()
    {
        TotalInputTokens = 0;
        TotalOutputTokens = 0;
        InvocationCount = 0;
        FailureCount = 0;
        TotalDuration = TimeSpan.Zero;
    }
}
