using Domain.Common;
using Flunt.Validations;

namespace Domain.Pipelines;

/// <summary>Nome de negócio da Pipeline (ex: "GenerateEnemQuestion").</summary>
internal sealed class PipelineName : ValueObject
{
    public string Value { get; private set; } = string.Empty;

    public PipelineName(string value)
    {
        AddNotifications(
            new Contract<PipelineName>()
                .IsNotNullOrEmpty(value, nameof(PipelineName), "Pipeline name cannot be empty")
                .IsLowerOrEqualsThan(value?.Length ?? 0, 200, nameof(PipelineName),
                    "Pipeline name cannot exceed 200 characters"));

        if (IsValid)
            Value = value!;
    }

    public override string ToString() => Value;
}
