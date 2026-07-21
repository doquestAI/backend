using Domain.Sessions.Aggregates;
using Domain.Sessions.ValueObjects;
using Domain.Shared.Repositories;

namespace Domain.Sessions.Repositories;

/// <summary>
/// Repositório do agregado AgentSession.
/// Gerencia histórico de sessões de agentes.
/// </summary>
public interface IAgentSessionRepository : IRepository<AgentSession>
{
    Task<AgentSession?> GetBySessionIdAsync(SessionId sessionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentSession>> GetActiveSessionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentSession>> GetExpiredSessionsAsync(CancellationToken cancellationToken = default);
}
