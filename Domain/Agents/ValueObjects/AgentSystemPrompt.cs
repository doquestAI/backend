using Domain.Common;
using Flunt.Validations;

namespace Domain.Agents.ValueObjects;

/// <summary>Instrução base do Agent (system prompt), validada por tamanho.</summary>
public sealed class AgentSystemPrompt : ValueObject
{
    public string? Value { get; private set; }

    public AgentSystemPrompt(string prompt)
    {
        AddNotifications(
            new Contract<AgentSystemPrompt>()
                .IsNotNullOrEmpty(prompt, nameof(AgentSystemPrompt),
                    "System prompt cannot be empty")
                .IsLowerOrEqualsThan(prompt?.Length ?? 0, 32000, nameof(AgentSystemPrompt),
                    "System prompt cannot exceed 32000 characters"));

        if (IsValid)
            Value = prompt;
    }

    public override string ToString() => Value ?? string.Empty;
}
