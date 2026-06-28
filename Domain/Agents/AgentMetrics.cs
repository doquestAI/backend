using Domain.Pipelines;

namespace Domain.Agents;

/// <summary>
/// Métricas acumuladas de um Agent ao longo do seu ciclo de vida (process-lifetime).
/// Cada invocação contribui para <see cref="TotalTokens"/>, <see cref="TotalLatency"/>
/// e <see cref="TotalInvocations"/>. Atualizada pelo próprio Agent após cada chamada.
/// </summary>
internal sealed class AgentMetrics
{
    public int TotalInvocations { get; private set; }
    public TokenUsage TotalTokens { get; private set; } = TokenUsage.Empty;
    public TimeSpan TotalLatency { get; private set; } = TimeSpan.Zero;
    public DateTime? LastInvokedAt { get; private set; }

    public void RecordInvocation(TokenUsage tokens, TimeSpan latency)
    {
        TotalInvocations++;
        TotalTokens = TotalTokens.Add(tokens);
        TotalLatency = TotalLatency.Add(latency);
        LastInvokedAt = DateTime.UtcNow;
    }
}
