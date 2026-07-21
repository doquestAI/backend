namespace Domain.Sessions.ValueObjects;

/// <summary>
/// Estado da sessão de um agente.
/// Lifecycle: Active → Paused ↔ Active → Closed
/// </summary>
public enum SessionState
{
    /// <summary>Sessão ativa, aceitando interações.</summary>
    Active = 0,

    /// <summary>Sessão pausada, sem novas interações.</summary>
    Paused = 1,

    /// <summary>Sessão finalizada, memória preservada para auditoria.</summary>
    Closed = 2,

    /// <summary>Sessão expirada por TTL.</summary>
    Expired = 3,
}
