using Domain.Shared.Core;

namespace Domain.Pipelines.ValueObjects;

/// <summary>
/// Nome de um passo dentro da pipeline.
/// </summary>
public sealed class StepName : ValueObject
{
    public string Value { get; private set; } = null!;

    public StepName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            AddNotification(nameof(StepName), "Step name cannot be empty");
            return;
        }

        if (value.Length > 256)
        {
            AddNotification(nameof(StepName), "Step name cannot exceed 256 characters");
            return;
        }

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is StepName other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
