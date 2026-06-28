using Domain.Common;
using Flunt.Validations;

namespace Domain.Agents.ValueObjects;

internal sealed class AgentName : ValueObject
{
    public string? Value { get; private set; }

    public AgentName(string name)
    {
        AddNotifications(
            new Contract<AgentName>()
                .IsNotNullOrEmpty(name, nameof(AgentName), "Agent name cannot be empty")
                .IsLowerOrEqualsThan(name?.Length ?? 0, 200, nameof(AgentName),
                    "Agent name cannot exceed 200 characters"));

        if (IsValid)
            Value = name;
    }

    public override string ToString() => Value ?? string.Empty;
}
