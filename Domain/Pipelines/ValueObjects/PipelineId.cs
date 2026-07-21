using Domain.Shared.Core;

namespace Domain.Pipelines.ValueObjects;

/// <summary>
/// Identificador único de uma pipeline.
/// </summary>
public sealed class PipelineId : ValueObject
{
    public Guid Value { get; }

    public PipelineId(Guid value)
    {
        if (value == Guid.Empty)
            AddNotification(nameof(PipelineId), "PipelineId cannot be empty");
        Value = value;
    }

    public static PipelineId New() => new(Guid.NewGuid());

    public override bool Equals(object? obj) =>
        obj is PipelineId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();
}
