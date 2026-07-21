using Domain.Shared.Core;

namespace Domain.Pipelines.ValueObjects;

/// <summary>
/// Nome semântico da pipeline (ex: "GenerateQuestion", "ExplainTopic").
/// </summary>
public sealed class PipelineName : ValueObject
{
    public string Value { get; private set; } = null!;

    public PipelineName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            AddNotification(nameof(PipelineName), "Pipeline name cannot be empty");
            return;
        }

        if (value.Length > 256)
        {
            AddNotification(nameof(PipelineName), "Pipeline name cannot exceed 256 characters");
            return;
        }

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is PipelineName other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
