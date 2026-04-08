using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Services.AI.Providers;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace Infrastructure.Services.AI;

/// <summary>
/// Creates and manages MAF agents and sessions for DoQuest.
/// </summary>
internal sealed class AgentFactory(
    IChatClient chatClient,
    IChatSessionRepository chatSessionRepository,
    IUnitOfWork unitOfWork)
{
    public ChatClientAgent CreateRagAgent(PostgresChatHistoryProvider historyProvider) =>
        new(chatClient, new ChatClientAgentOptions
        {
            ChatHistoryProvider = historyProvider
        });

    /// <summary>
    /// Creates or resumes a MAF AgentSession for the given chat session.
    /// Binds the history provider to this session's DB key.
    /// </summary>
    public async Task<AgentSession> GetOrCreateAgentSessionAsync(
        ChatClientAgent agent,
        ChatSession chatSession,
        PostgresChatHistoryProvider historyProvider,
        CancellationToken ct = default)
    {
        AgentSession agentSession;

        if (!string.IsNullOrWhiteSpace(chatSession.SerializedAgentSession))
        {
            try
            {
                var element = JsonSerializer.Deserialize<JsonElement>(chatSession.SerializedAgentSession);
                agentSession = await agent.DeserializeSessionAsync(element, cancellationToken: ct);
            }
            catch
            {
                // Fallback: create fresh session if deserialization fails
                agentSession = await agent.CreateSessionAsync(ct);
            }
        }
        else
        {
            agentSession = await agent.CreateSessionAsync(ct);
        }

        historyProvider.BindSession(agentSession, chatSession.Id);
        return agentSession;
    }

    /// <summary>
    /// Serializes the agent session state back and persists it in the ChatSession entity.
    /// </summary>
    public async Task PersistAgentSessionAsync(
        ChatClientAgent agent,
        AgentSession agentSession,
        ChatSession chatSession,
        CancellationToken ct = default)
    {
        var serialized = await agent.SerializeSessionAsync(agentSession, cancellationToken: ct);
        chatSession.StoreAgentSession(JsonSerializer.Serialize(serialized));

        chatSessionRepository.Update(chatSession);
        await unitOfWork.CommitAsync(ct);
    }
}
