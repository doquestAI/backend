using System.Collections.Concurrent;

namespace Domain.Pipelines;

/// <summary>
/// Métricas agregadas da Pipeline inteira. Acumula contagem de tokens, latência e número
/// de chamadas ao LLM através de todos os steps. Detalha o consumo por agente.
/// </summary>
internal sealed class PipelineMetrics
{
    public int StepsExecuted { get; private set; }
    public TokenUsage TotalTokens { get; private set; } = TokenUsage.Empty;
    public TimeSpan TotalDuration { get; private set; } = TimeSpan.Zero;
    public int TotalLlmCalls { get; private set; }

    private readonly ConcurrentDictionary<string, TokenUsage> _byAgent = new();
    public IReadOnlyDictionary<string, TokenUsage> TokensByAgent => _byAgent;

    public void Accumulate(StepMetrics metrics)
    {
        StepsExecuted++;
        TotalTokens = TotalTokens.Add(metrics.Tokens);
        TotalDuration = TotalDuration.Add(metrics.Duration);
        TotalLlmCalls += metrics.LlmCallCount;
    }

    public void AttributeTo(string agentName, TokenUsage usage)
    {
        _byAgent.AddOrUpdate(agentName, usage, (_, current) => current.Add(usage));
    }
}
