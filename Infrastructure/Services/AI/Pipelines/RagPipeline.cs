using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services.AI.Embeddings;
using Domain.Records;
using Infrastructure.Interfaces.AI.Pipelines;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Infrastructure.Services.AI.Pipelines;

internal sealed class RagPipeline(
    IChatClient chatClient,
    IEmbeddingService embeddingService,
    IDocumentRepository documentRepository) : IRagPipeline
{
    private const string QueryRewriteInstructions =
        """
        Você é um assistente que reformula perguntas para otimizar a busca semântica em uma base de conhecimento sobre vestibulares brasileiros.
        Dado o histórico de conversa e a pergunta do usuário, reformule a pergunta como uma consulta de busca clara e standalone.
        Retorne APENAS a consulta reformulada, sem explicações adicionais.
        """;

    private const string RagResponseInstructions =
        """
        Você é o DoQuest, um assistente educacional especializado em vestibulares brasileiros (ENEM, FUVEST, Unicamp, UNESP e outros).

        Diretrizes:
        1. Responda SEMPRE em Português Brasileiro.
        2. Base suas respostas NO CONTEXTO FORNECIDO. Se o contexto não for suficiente, diga claramente que não encontrou informações suficientes sobre o assunto.
        3. Seja didático, claro e objetivo — o público são estudantes do Ensino Médio.
        4. Quando citar informações do contexto, mencione a fonte (ex: "De acordo com o material de estudo...").
        5. Para questões de múltipla escolha, explique o raciocínio por trás da resposta correta.
        6. Nunca invente fatos ou dados que não estejam no contexto fornecido.
        """;

    public async Task<(string Reply, RagContext Context)> ExecuteAsync(
        string userMessage,
        Guid? vestibularId,
        AgentSession agentSession,
        IChatClient? queryRewriteClient = null,
        CancellationToken ct = default)
    {
        // Step 1: Rewrite query for better retrieval
        var rewrittenQuery = await RewriteQueryAsync(userMessage, queryRewriteClient ?? chatClient, ct);

        // Step 2: Generate embedding from rewritten query
        var embedding = await embeddingService.GenerateAsync(rewrittenQuery, ct);

        // Step 3: Semantic search via pgvector
        var chunks = await documentRepository.SearchSimilarChunksAsync(embedding, topK: 5, vestibularId, ct);
        var ragContext = new RagContext(chunks);

        // Step 4: Generate answer with RAG context injected via TextSearchProvider
        var ragContextString = ragContext.ToContextString();

        var textSearchProvider = new TextSearchProvider(
            (_, _) => Task.FromResult<IEnumerable<TextSearchProvider.TextSearchResult>>(
            [
                new TextSearchProvider.TextSearchResult
                {
                    SourceName = "DoQuest Knowledge Base",
                    Text = string.IsNullOrWhiteSpace(ragContextString)
                        ? "Nenhum contexto relevante encontrado para esta pergunta."
                        : ragContextString
                }
            ]),
            new TextSearchProviderOptions
            {
                SearchTime = TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke,
                RecentMessageMemoryLimit = 6
            });

        var responseAgent = chatClient.AsAIAgent(new ChatClientAgentOptions
        {
            ChatOptions = new ChatOptions { Instructions = RagResponseInstructions },
            AIContextProviders = [textSearchProvider]
        });

        var reply = await responseAgent.RunAsync(userMessage, agentSession, null, ct);
        return (reply?.Text ?? string.Empty, ragContext);
    }

    private async Task<string> RewriteQueryAsync(string userMessage, IChatClient client, CancellationToken ct)
    {
        try
        {
            var rewriteAgent = client.AsAIAgent(new ChatClientAgentOptions
            {
                ChatOptions = new ChatOptions { Instructions = QueryRewriteInstructions }
            });

            var session = await rewriteAgent.CreateSessionAsync(ct);
            var result = await rewriteAgent.RunAsync(userMessage, session, null, ct);
            var rewritten = result?.Text?.Trim();

            return string.IsNullOrWhiteSpace(rewritten) ? userMessage : rewritten;
        }
        catch
        {
            return userMessage;
        }
    }
}
