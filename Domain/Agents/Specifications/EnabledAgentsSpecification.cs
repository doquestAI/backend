using Domain.Agents.Aggregates;
using Domain.Shared.Core;

namespace Domain.Agents.Specifications;

/// <summary>
/// Specification para agents habilitados.
/// Encapsula critério de query para não repetir em múltiplos repositórios.
/// </summary>
public sealed class EnabledAgentsSpecification : Specification<Agent>
{
    public EnabledAgentsSpecification()
    {
        Criteria = agent => agent.IsEnabled && agent.DeletedAt == null;
    }
}
