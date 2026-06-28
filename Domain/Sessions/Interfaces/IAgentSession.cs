using Domain.Sessions.ValueObjects;

namespace Domain.Sessions.Interfaces;

/// <summary>Contrato para hidratação/persistência de <see cref="AgentSession"/>.</summary>
public interface IAgentSessionStore
{
    Task<AgentSession?> GetAsync(SessionId id, CancellationToken cancellationToken = default);
    Task SaveAsync(AgentSession session, CancellationToken cancellationToken = default);
    Task RemoveAsync(SessionId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(SessionId id, CancellationToken cancellationToken = default);
}
