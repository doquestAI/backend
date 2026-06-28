using Domain.Common;

namespace Domain.Pipelines;

/// <summary>Identidade única de uma Pipeline (instância de execução).</summary>
public sealed class PipelineId : ValueObject
{
    public Guid Value { get; }

    private PipelineId(Guid value) => Value = value;

    public static PipelineId New() => new(Guid.NewGuid());
    public static PipelineId From(Guid value) => new(value);

    public override string ToString() => Value.ToString("N");
}
