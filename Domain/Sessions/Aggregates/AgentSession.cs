using Domain.Agents.ValueObjects;
using Domain.Sessions.ValueObjects;
using Domain.Shared.Core;

namespace Domain.Sessions.Aggregates;

/// <summary>
/// AGGREGATE ROOT: AgentSession
/// Representa uma conversa/interação de um usuário com um agente.
/// Encapsula memória de conversa, histórico de execução, estado da sessão.
/// Nunca deleta memória (auditoria de interações).
/// </summary>
public sealed class AgentSession : AggregateRoot
{
    private readonly List<MemoryEntry> _memoryEntries = [];
    private readonly List<ExecutionRecord> _executionHistory = [];

    public SessionId SessionId { get; private set; } = null!;
    public AgentId AgentId { get; private set; } = null!;
    public Guid? UserId { get; private set; }
    public SessionState State { get; private set; }
    public IReadOnlyList<MemoryEntry> MemoryEntries => _memoryEntries.AsReadOnly();
    public IReadOnlyList<ExecutionRecord> ExecutionHistory => _executionHistory.AsReadOnly();
    public int TurnCount => _memoryEntries.Count(e => e.Role == MemoryRole.User);
    public DateTime StartedAt { get; private set; }
    public DateTime? PausedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    private AgentSession() { }

    public static AgentSession Create(AgentId agentId, Guid? userId = null, TimeSpan? ttl = null)
    {
        var session = new AgentSession
        {
            Id = Guid.NewGuid(),
            SessionId = SessionId.New(),
            AgentId = agentId,
            UserId = userId,
            State = SessionState.Active,
            StartedAt = DateTime.UtcNow,
            ExpiresAt = ttl.HasValue ? DateTime.UtcNow.Add(ttl.Value) : null,
        };

        session.RaiseDomainEvent(new SessionCreatedEvent(session.Id, agentId.Value, userId));
        return session;
    }

    public static AgentSession CreateWithId(AgentId agentId, SessionId sessionId, Guid? userId = null, TimeSpan? ttl = null)
    {
        var session = new AgentSession
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            AgentId = agentId,
            UserId = userId,
            State = SessionState.Active,
            StartedAt = DateTime.UtcNow,
            ExpiresAt = ttl.HasValue ? DateTime.UtcNow.Add(ttl.Value) : null,
        };

        session.RaiseDomainEvent(new SessionCreatedEvent(session.Id, agentId.Value, userId));
        return session;
    }

    /// <summary>Adiciona entrada à memória de conversa.</summary>
    public void AddMemoryEntry(MemoryRole role, string content, string? name = null)
    {
        if (State != SessionState.Active)
        {
            AddNotification(nameof(State), "Cannot add memory entry to inactive session");
            return;
        }

        var entry = new MemoryEntry(role, content, name);
        _memoryEntries.Add(entry);

        RaiseDomainEvent(new MemoryEntryAddedEvent(Id, SessionId.Value, role));
    }

    /// <summary>Registra execução de uma ação no histórico.</summary>
    public void RecordExecution(string actionName, bool success, TimeSpan duration, string? errorMessage = null)
    {
        var record = new ExecutionRecord(actionName, success, duration, errorMessage);
        _executionHistory.Add(record);

        if (!success)
            RaiseDomainEvent(new ExecutionFailedEvent(Id, SessionId.Value, actionName, errorMessage));
    }

    /// <summary>Retorna últimas N entradas de memória.</summary>
    public IReadOnlyList<MemoryEntry> GetLastMemoryEntries(int count) =>
        _memoryEntries.TakeLast(count).ToList();

    /// <summary>Pausa a sessão (sem perder memória).</summary>
    public void Pause()
    {
        if (State != SessionState.Active)
        {
            AddNotification(nameof(State), "Only active sessions can be paused");
            return;
        }

        State = SessionState.Paused;
        PausedAt = DateTime.UtcNow;
        RaiseDomainEvent(new SessionPausedEvent(Id, SessionId.Value));
    }

    /// <summary>Retoma sessão pausada.</summary>
    public void Resume()
    {
        if (State != SessionState.Paused)
        {
            AddNotification(nameof(State), "Only paused sessions can be resumed");
            return;
        }

        State = SessionState.Active;
        PausedAt = null;
        RaiseDomainEvent(new SessionResumedEvent(Id, SessionId.Value));
    }

    /// <summary>Finaliza sessão (preserva memória para auditoria).</summary>
    public void Close()
    {
        if (State == SessionState.Closed)
            return;

        State = SessionState.Closed;
        ClosedAt = DateTime.UtcNow;
        RaiseDomainEvent(new SessionClosedEvent(Id, SessionId.Value));
    }

    /// <summary>Verifica se sessão expirou por TTL.</summary>
    public bool IsExpired() =>
        ExpiresAt.HasValue && DateTime.UtcNow >= ExpiresAt.Value;

    /// <summary>Verifica se sessão ainda está válida.</summary>
    public bool IsActive() =>
        State == SessionState.Active && !IsExpired();

    /// <summary>Limpa memória (ação irreversível).</summary>
    public void ClearMemory()
    {
        if (_memoryEntries.Count == 0)
            return;

        _memoryEntries.Clear();
        RaiseDomainEvent(new SessionMemoryClearedEvent(Id, SessionId.Value));
    }
}

/// <summary>
/// Registro de uma execução dentro da sessão.
/// Rastreabilidade de o que aconteceu e quando.
/// </summary>
public sealed class ExecutionRecord
{
    public string ActionName { get; }
    public bool Success { get; }
    public TimeSpan Duration { get; }
    public string? ErrorMessage { get; }
    public DateTime ExecutedAt { get; }

    public ExecutionRecord(string actionName, bool success, TimeSpan duration, string? errorMessage = null)
    {
        ActionName = actionName;
        Success = success;
        Duration = duration;
        ErrorMessage = errorMessage;
        ExecutedAt = DateTime.UtcNow;
    }
}
