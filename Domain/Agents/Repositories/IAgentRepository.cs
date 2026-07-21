using Domain.Agents.Aggregates;
using Domain.Agents.ValueObjects;
using Domain.Shared.Repositories;

namespace Domain.Agents.Repositories;

/// <summary>
/// Repositório do agregado Agent.
/// Trabalha exclusivamente com Agent (raiz agregada), nunca com suas entidades internas.
/// </summary>
public interface IAgentRepository : IRepository<Agent>
{
    Task<Agent?> GetByNameAsync(AgentName name, CancellationToken cancellationToken = default);
    Task<Agent?> GetByAgentIdAsync(AgentId agentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Agent>> GetByRoleAsync(AgentRole role, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Agent>> GetEnabledAsync(CancellationToken cancellationToken = default);
}
