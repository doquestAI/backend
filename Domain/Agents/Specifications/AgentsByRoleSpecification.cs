using Domain.Agents.Aggregates;
using Domain.Agents.ValueObjects;
using Domain.Shared.Core;

namespace Domain.Agents.Specifications;

/// <summary>
/// Specification para agents por role.
/// </summary>
public sealed class AgentsByRoleSpecification : Specification<Agent>
{
    public AgentsByRoleSpecification(AgentRole role)
    {
        Criteria = agent => agent.Role == role && agent.DeletedAt == null;
    }
}
