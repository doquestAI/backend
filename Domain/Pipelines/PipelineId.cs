using Domain.Common;

namespace Domain.Pipelines;

/// <summary>Identidade única de uma Pipeline (instância de execução).</summary>
internal sealed class PipelineId : ValueObject
{
    public Guid Value { get; }

    private PipelineId(Guid value) => Value = value;

    internal static PipelineId New() => new(Guid.NewGuid());
    internal static PipelineId From(Guid value) => new(value);

    public override string ToString() => Value.ToString("N");
}
