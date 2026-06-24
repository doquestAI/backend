using AI.Providers.Abstractions;
using Domain.Enums;
using Domain.Interfaces.Agents;
using Domain.ValueObjects;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AI.Agents.Enem;

/// <summary>Explica teoria de um tópico do ENEM em detalhes.</summary>
internal sealed class ExplainerAgent(
    IChatClient chatClient,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGen,
    IVectorStore vectorStore,
    [FromKeyedServices(PromptProvider.File)] IPromptProvider promptProvider,
    IAgentSession session,
    IAgentMemory memory,
    IOptions<AgentOptions> options,
    ILogger<ExplainerAgent> logger)
    : EnemTextAgent<ExplainRequest>(chatClient, embeddingGen, vectorStore, promptProvider,
                                    session, memory, options, logger)
{
    protected override string AgentKey => "enem-explainer";
    protected override string KnowledgeArea => _currentArea;

    private string _currentArea = "Geral";

    protected override Task<ExplainRequest> PreProcessAsync(ExplainRequest input, CancellationToken ct)
    {
        _currentArea = input.Area;
        return Task.FromResult(input);
    }

    protected override string ExtractTextFromInput(ExplainRequest input)
        => $"Explique detalhadamente sobre: {input.Topic}. Área: {input.Area}";
}