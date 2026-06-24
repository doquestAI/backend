using AI.Agents.Base;
using AI.Providers.Abstractions;
using Domain.Interfaces.Agents;
using Domain.ValueObjects;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AI.Agents.Enem;

/// <summary>
/// Base para todos os agentes do domínio ENEM.
///
/// Adiciona ao AgentBase:
///   - Coleção RAG dedicada ao ENEM ("enem-knowledge")
///   - Variáveis comuns de template (knowledge_area, enem_year)
/// </summary>
public abstract class EnemAgentBase<TIn, TOut>(
    IChatClient chatClient,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGen,
    IVectorStore vectorStore,
    IPromptProvider promptProvider,
    IAgentSession session,
    IAgentMemory memory,
    IOptions<AgentOptions> options,
    ILogger logger) : AgentBase<TIn, TOut>(chatClient, embeddingGen, vectorStore, promptProvider,
           session, memory, options, logger)
{
    protected override string RagCollection => "enem-knowledge";

    protected virtual string KnowledgeArea => "Geral";
    protected virtual int EnemYear => DateTime.UtcNow.Year;

    protected override IReadOnlyDictionary<string, string> GetPromptVariables()
        => new Dictionary<string, string>(base.GetPromptVariables())
        {
            ["knowledge_area"] = KnowledgeArea,
            ["enem_year"] = EnemYear.ToString(),
        };
}

/// <summary>
/// Especialização para agentes cuja saída é string (maioria dos casos ENEM).
/// </summary>
public abstract class EnemTextAgent<TIn>(
    IChatClient chatClient,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGen,
    IVectorStore vectorStore,
    IPromptProvider promptProvider,
    IAgentSession session,
    IAgentMemory memory,
    IOptions<AgentOptions> options,
    ILogger logger) : EnemAgentBase<TIn, string>(chatClient, embeddingGen, vectorStore, promptProvider,
           session, memory, options, logger)
{
    protected override Task<string> ParseResponseAsync(
        ChatCompletion response, CancellationToken ct)
        => Task.FromResult(response.Message.Text ?? string.Empty);
}