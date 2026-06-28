using Domain.Agents;
using Domain.Agents.ValueObjects;
using Domain.Capabilities;
using Domain.Pipelines;

namespace Domain.Interfaces.Agents;

/// <summary>
/// Contrato de domínio do Agent. AgentBase (camada AI) implementa esta interface
/// e também herda do <c>ChatClientAgent</c> do MAF — então pode ser invocado tanto
/// pela API alto-nível desta interface quanto pelos métodos nativos do framework.
/// </summary>
internal interface IAgent : IAgentAware
{
    /// <summary>
    /// Invoca o agente com um prompt textual. Retorna a resposta com métricas
    /// (tokens, latência). Suporta sessão (histórico) e capabilities de runtime.
    /// </summary>
    Task<AgentInvocationResult> InvokeAsync(
        AgentInvocationInput input,
        CancellationToken cancellationToken = default);

    /// <summary>Invocação em streaming — yield de chunks de texto conforme chegam do LLM.</summary>
    IAsyncEnumerable<string> InvokeStreamingAsync(
        AgentInvocationInput input,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Auto-consciência do Agent: identidade, papel, status, capabilities permanentes
/// e métricas acumuladas. Permite à Pipeline e à camada Application inspecionar
/// o estado de qualquer Agent sem depender da implementação.
/// </summary>
internal interface IAgentAware
{
    AgentId AgentId { get; }
    AgentName AgentName { get; }
    AgentRole Role { get; }
    AgentStatus Status { get; }
    AgentCapabilities Capabilities { get; }
    AgentMetrics Metrics { get; }
}

/// <summary>Input de uma invocação ao agent.</summary>
internal sealed record AgentInvocationInput(
    string Prompt,
    string? SessionKey = null,
    IReadOnlyList<PluginDescriptor>? RuntimePlugins = null,
    IReadOnlyList<McpDescriptor>? RuntimeMcps = null,
    bool UseRag = false);

/// <summary>Saída de uma invocação ao agent.</summary>
internal sealed record AgentInvocationResult(
    string Text,
    TokenUsage Tokens,
    TimeSpan Latency);
