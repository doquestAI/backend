using Domain.Shared.Core;

namespace Domain.Agents.ValueObjects;

/// <summary>
/// System prompt que instrui o agente (LLM).
/// Imutável, validado quanto a tamanho máximo (32k chars = limite Claude).
/// </summary>
public sealed class AgentSystemPrompt : ValueObject
{
    private const int MaxLength = 32000;

    public string Value { get; private set; } = null!;

    public AgentSystemPrompt(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            AddNotification(nameof(AgentSystemPrompt), "System prompt cannot be empty");
            return;
        }

        if (prompt.Length > MaxLength)
        {
            AddNotification(nameof(AgentSystemPrompt),
                $"System prompt cannot exceed {MaxLength} characters");
            return;
        }

        Value = prompt;
    }

    public override bool Equals(object? obj) =>
        obj is AgentSystemPrompt other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
