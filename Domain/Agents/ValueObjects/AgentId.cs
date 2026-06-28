using Domain.Common;

namespace Domain.Agents.ValueObjects;

/// <summary>Identidade única do Agent (GUID-based).</summary>
internal sealed class AgentId : ValueObject
{
    public Guid Value { get; }

    private AgentId(Guid value) => Value = value;

    internal static AgentId New() => new(Guid.NewGuid());

    internal static AgentId From(Guid value) => new(value);

    public override string ToString() => Value.ToString("N");
}
