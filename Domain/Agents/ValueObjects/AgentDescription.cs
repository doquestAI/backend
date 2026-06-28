using Domain.Common;
using Flunt.Validations;

namespace Domain.Agents.ValueObjects;

internal sealed class AgentDescription : ValueObject
{
    public string? Value { get; private set; }

    public AgentDescription(string description)
    {
        AddNotifications(
            new Contract<AgentDescription>()
                .IsNotNullOrEmpty(description, nameof(AgentDescription),
                    "Agent description cannot be empty")
                .IsLowerOrEqualsThan(description?.Length ?? 0, 1000, nameof(AgentDescription),
                    "Agent description cannot exceed 1000 characters"));

        if (IsValid)
            Value = description;
    }

    public override string ToString() => Value ?? string.Empty;
}
