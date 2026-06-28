using Domain.Agents;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AI.Common.Config;

/// <summary>
/// Configuração estática que define a identidade e o comportamento de um Agent.
/// Usada pelo construtor de <c>AgentBase</c> para instanciar tanto a parte MAF
/// (<see cref="ChatClientAgent"/>) quanto as propriedades de negócio.
/// </summary>
internal sealed class AgentConfig
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required AgentRole Role { get; init; }
    public required string SystemPrompt { get; init; }
    public float Temperature { get; init; } = 0.7f;
    public int MaxOutputTokens { get; init; } = 1024;

    /// <summary>Capabilities semânticas (flags) — pertencem à identidade do Agent.</summary>
    public AgentCapabilities? Capabilities { get; init; }

    /// <summary>Providers MAF que injetam contexto/memória antes de cada chamada LLM.</summary>
    public IList<AIContextProvider>? ContextProviders { get; init; }
}
