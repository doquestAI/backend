using Domain.Interfaces.Services;
using Infrastructure.Options;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;

namespace Infrastructure.AI;

internal sealed class OllamaChatAgentService : IChatAgentService
{
    private readonly IChatClient _chatClient;

    public OllamaChatAgentService(IOptions<OllamaOptions> options)
    {
        var opts = options.Value;
        _chatClient = new OllamaApiClient(new Uri(opts.Endpoint))
            .AsChatClient(opts.ChatModel);
    }

    public async Task<string> ChatAsync(
        string userMessage,
        string ragContext,
        IEnumerable<ChatMessage> conversationHistory,
        CancellationToken cancellationToken = default)
    {
        // Build system prompt with RAG context injected
        var systemPrompt = BuildSystemPrompt(ragContext);

        // Configure agent with TextSearchProvider that returns the pre-retrieved context
        var textSearchProvider = new TextSearchProvider(
            searchFunction: (query, ct) =>
            Task.FromResult<IEnumerable<TextSearchProvider.TextSearchResult>>(
            [
                new TextSearchProvider.TextSearchResult
                {
                    SourceName = "DoQuest RAG",
                    Text = string.IsNullOrWhiteSpace(ragContext)
                        ? "Nenhum contexto relevante encontrado para esta pergunta."
                        : ragContext
                }
            ]),
            options: new TextSearchProviderOptions
            {
                SearchTime = TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke,
                RecentMessageMemoryLimit = 6
            });

        var agent = new ChatClientAgent(_chatClient, new ChatClientAgentOptions
        {
            ChatOptions = new ChatOptions
            {
                Instructions = systemPrompt
            },
            AIContextProviders = [textSearchProvider]
        });

        // Build message list (history + current message)
        var messages = conversationHistory.ToList();
        messages.Add(new ChatMessage(ChatRole.User, userMessage));

        // Create session for this request
        var session = await agent.CreateSessionAsync(cancellationToken);

        // Run the agent
        var response = await agent.RunAsync(userMessage, session, cancellationToken);

        return response?.Text ?? string.Empty;
    }

    private static string BuildSystemPrompt(string ragContext) =>
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
}
