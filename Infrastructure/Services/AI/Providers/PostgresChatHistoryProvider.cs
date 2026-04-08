using Domain.Enums;
using Domain.Interfaces.Repositories;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using DomainMessage = Domain.Entities.ChatMessage;

namespace Infrastructure.Services.AI.Providers;

internal sealed class PostgresChatHistoryProvider(
    IChatSessionRepository chatSessionRepository,
    IUnitOfWork unitOfWork,
    int messageWindowSize = 20) : ChatHistoryProvider
{
    private sealed class SessionState
    {
        public Guid ChatSessionId { get; set; }
    }

    private readonly ProviderSessionState<SessionState> _sessionState =
        new(_ => new SessionState(), nameof(PostgresChatHistoryProvider));

    protected override async ValueTask<IEnumerable<ChatMessage>> ProvideChatHistoryAsync(
        InvokingContext context,
        CancellationToken cancellationToken = default)
    {
        var state = _sessionState.GetOrInitializeState(context.Session);
        if (state.ChatSessionId == Guid.Empty)
            return [];

        var session = await chatSessionRepository.GetWithMessagesAsync(
            state.ChatSessionId, messageWindowSize, cancellationToken);

        if (session is null)
            return [];

        return session.Messages
            .OrderBy(m => m.CreatedAt)
            .Select(m => new ChatMessage(
                m.Role switch
                {
                    ChatMessageRole.User => ChatRole.User,
                    ChatMessageRole.Assistant => ChatRole.Assistant,
                    ChatMessageRole.System => ChatRole.System,
                    _ => ChatRole.User
                },
                m.Content));
    }

    protected override async ValueTask StoreChatHistoryAsync(
        InvokedContext context,
        CancellationToken cancellationToken = default)
    {
        var state = _sessionState.GetOrInitializeState(context.Session);
        if (state.ChatSessionId == Guid.Empty)
            return;

        var newMessages = new List<DomainMessage>();

        foreach (var msg in context.RequestMessages)
        {
            var role = MapRole(msg.Role);
            newMessages.Add(DomainMessage.Create(state.ChatSessionId, role, msg.Text ?? string.Empty));
        }

        foreach (var msg in context.ResponseMessages ?? [])
        {
            var role = MapRole(msg.Role);
            newMessages.Add(DomainMessage.Create(state.ChatSessionId, role, msg.Text ?? string.Empty));
        }

        if (newMessages.Count == 0)
            return;

        await chatSessionRepository.AddMessagesAsync(newMessages, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    /// <summary>
    /// Binds this provider to a specific chat session so it knows where to load/store history.
    /// Call this before running the agent for a given session.
    /// </summary>
    public void BindSession(AgentSession agentSession, Guid chatSessionId)
    {
        var state = _sessionState.GetOrInitializeState(agentSession);
        state.ChatSessionId = chatSessionId;
        _sessionState.SaveState(agentSession, state);
    }

    private static ChatMessageRole MapRole(ChatRole role)
    {
        if (role == ChatRole.User)
            return ChatMessageRole.User;
        if (role == ChatRole.Assistant)
            return ChatMessageRole.Assistant;
        if (role == ChatRole.System)
            return ChatMessageRole.System;
        return ChatMessageRole.User;
    }
}
