using Domain.Agents.ValueObjects;
using Domain.Sessions.ValueObjects;
using Flunt.Notifications;

namespace Domain.Sessions;

/// <summary>
/// Sessão de execução de um Agent — agrupa estado, histórico de eventos e
/// memória de conversa. Esta é a contraparte de domínio do <c>AgentSession</c>
/// nativo do MAF, mantida pura (sem referência ao framework).
/// </summary>
internal class AgentSession : Notifiable<Notification>
{
    public SessionId Id { get; private set; } = default!;
    public AgentId AgentId { get; private set; } = default!;
    public SessionState State { get; private set; }
    public ExecutionHistory History { get; private set; } = default!;
    public ConversationMemory Memory { get; private set; } = default!;
    public DateTime StartedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    private AgentSession() { }

    public AgentSession(AgentId agentId, TimeSpan? ttl = null)
    {
        if (agentId is null)
        {
            AddNotification(nameof(AgentId), "AgentId is required to create a session");
            return;
        }

        Id = SessionId.New();
        AgentId = agentId;
        State = SessionState.Active;
        History = new ExecutionHistory();
        Memory = new ConversationMemory();
        StartedAt = DateTime.UtcNow;
        ExpiresAt = ttl.HasValue ? DateTime.UtcNow.Add(ttl.Value) : null;
    }

    public AgentSession(AgentId agentId, SessionId sessionId, TimeSpan? ttl = null)
    {
        if (agentId is null)
        {
            AddNotification(nameof(AgentId), "AgentId is required to create a session");
            return;
        }
        if (sessionId is null || !sessionId.IsValid)
        {
            AddNotification(nameof(SessionId), "SessionId is invalid");
            return;
        }

        Id = sessionId;
        AgentId = agentId;
        State = SessionState.Active;
        History = new ExecutionHistory();
        Memory = new ConversationMemory();
        StartedAt = DateTime.UtcNow;
        ExpiresAt = ttl.HasValue ? DateTime.UtcNow.Add(ttl.Value) : null;
    }

    public void Pause()
    {
        if (State != SessionState.Active)
        {
            AddNotification(nameof(State), "Only active sessions can be paused");
            return;
        }
        State = SessionState.Paused;
    }

    public void Resume()
    {
        if (State != SessionState.Paused)
        {
            AddNotification(nameof(State), "Only paused sessions can be resumed");
            return;
        }
        State = SessionState.Active;
    }

    public void Close()
    {
        State = SessionState.Closed;
    }

    public bool IsExpired() => ExpiresAt.HasValue && DateTime.UtcNow >= ExpiresAt.Value;
}
