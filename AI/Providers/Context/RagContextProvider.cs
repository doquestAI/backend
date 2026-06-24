using AI.Providers.Abstractions;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace AI.Providers.Context;

/// <summary>
/// <see cref="AIContextProvider"/> que injeta contexto RAG antes de cada invocação do agente.
///
/// Fluxo (chamado em <c>InvokingAsync</c>):
///   1. Extrai texto da última mensagem do usuário em <see cref="AIContextProvider.InvokingContext.Messages"/>
///   2. Gera embedding via <see cref="IEmbeddingGenerator{TInput, TEmbedding}"/>
///   3. Busca top-K chunks no <see cref="IVectorStore"/> (coleção configurável)
///   4. Retorna um <see cref="AIContext"/> com chunks formatados em <see cref="AIContext.Instructions"/>
///
/// Substitui o método hardcoded <c>RetrieveRagContextAsync</c> do antigo <c>AgentBase</c>.
/// </summary>
public sealed class RagContextProvider(
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    IVectorStore vectorStore,
    string collection,
    ILogger<RagContextProvider> logger,
    int topK = 5,
    float threshold = 0.75f)
    : AIContextProvider
{
    protected override async ValueTask<AIContext> ProvideAIContextAsync(
        InvokingContext context,
        CancellationToken cancellationToken = default)
    {
        var queryText = ExtractLastUserText(context.AIContext.Messages);
        if (string.IsNullOrWhiteSpace(queryText))
            return new AIContext();

        try
        {
            var embedding = await embeddingGenerator.GenerateAsync(
                [queryText], cancellationToken: cancellationToken);

            var vector = embedding.FirstOrDefault()?.Vector ?? ReadOnlyMemory<float>.Empty;
            if (vector.IsEmpty)
                return new AIContext();

            var results = await vectorStore.SearchAsync(
                collection, vector, topK, threshold, cancellationToken);

            if (results.Count == 0)
                return new AIContext();

            var formatted = string.Join("\n\n", results
                .OrderByDescending(r => r.Score)
                .Select((r, i) => $"[{i + 1}] {r.Content}"));

            logger.LogDebug(
                "RAG: {Count} chunks recuperados da coleção {Collection} para query '{Query}'.",
                results.Count, collection, queryText[..Math.Min(60, queryText.Length)]);

            return new AIContext
            {
                Instructions = $"### Contexto recuperado (RAG)\n{formatted}",
            };
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "RAG falhou para coleção {Collection}, prosseguindo sem contexto.", collection);
            return new AIContext();
        }
    }

    private static string ExtractLastUserText(IEnumerable<ChatMessage>? messages)
    {
        if (messages is null)
            return string.Empty;
        var last = messages.LastOrDefault(m => m.Role == ChatRole.User);
        return last?.Text ?? string.Empty;
    }
}
