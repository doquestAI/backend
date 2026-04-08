using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services.AI;
using Infrastructure.Interfaces.AI.Pipelines;
using Infrastructure.Options;
using Infrastructure.Services.AI;
using Infrastructure.Services.AI.Providers;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;
using System.Runtime.CompilerServices;

namespace Infrastructure.AI;

internal sealed class OllamaChatAgentService(
    IOptions<OllamaOptions> options,
    IChatSessionRepository chatSessionRepository,
    IRagPipeline ragPipeline,
    IUnitOfWork unitOfWork) : IChatAgentService
{
    private IChatClient BuildChatClient()
    {
        var opts = options.Value;
        return new OllamaApiClient(new Uri(opts.Endpoint), opts.ChatModel);
    }

    public async Task<ChatAgentResponse> ProcessMessageAsync(
        Guid sessionId,
        string userMessage,
        Guid? vestibularId,
        CancellationToken ct = default)
    {
        var chatClient = BuildChatClient();

        var chatSession = await chatSessionRepository.GetWithMessagesAsync(sessionId, messageLimit: 0, ct)
            ?? throw new InvalidOperationException($"Chat session {sessionId} not found.");

        var historyProvider = new PostgresChatHistoryProvider(chatSessionRepository, unitOfWork);
        var factory = new AgentFactory(chatClient, chatSessionRepository, unitOfWork);

        var agent = factory.CreateRagAgent(historyProvider);
        var agentSession = await factory.GetOrCreateAgentSessionAsync(agent, chatSession, historyProvider, ct);

        var (reply, ragContext) = await ragPipeline.ExecuteAsync(
            userMessage, vestibularId, agentSession, ct: ct);

        await factory.PersistAgentSessionAsync(agent, agentSession, chatSession, ct);

        return new ChatAgentResponse(reply, ragContext.Chunks.Count, sessionId);
    }

    public async IAsyncEnumerable<string> StreamMessageAsync(
        Guid sessionId,
        string userMessage,
        Guid? vestibularId,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var response = await ProcessMessageAsync(sessionId, userMessage, vestibularId, ct);
        yield return response.Reply;
    }
}
