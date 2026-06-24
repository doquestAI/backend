using AI.Providers.Abstractions;
using Domain.Enums;
using Domain.Interfaces.Agents;
using Domain.ValueObjects;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AI.Agents.Enem;

/// <summary>Responde dúvidas gerais sobre o ENEM.</summary>
internal sealed class HelperEnemAgent(
    IChatClient chatClient,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGen,
    IVectorStore vectorStore,
    [FromKeyedServices(PromptProvider.File)] IPromptProvider promptProvider,
    IAgentSession session,
    IAgentMemory memory,
    IOptions<AgentOptions> options,
    ILogger<HelperEnemAgent> logger)
    : EnemTextAgent<string>(chatClient, embeddingGen, vectorStore, promptProvider,
                             session, memory, options, logger)
{
    protected override string AgentKey => "enem-helper";

    protected override async Task SaveToMemoryAsync(
        string input, string response, CancellationToken ct)
    {
        var key = $"doubt:{input[..Math.Min(60, input.Length)]}";
        await Memory.SaveFactAsync(key, $"Pergunta: {input}\nResposta: {response}", ct);
    }
}