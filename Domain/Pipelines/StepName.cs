using Domain.Common;
using Flunt.Validations;

namespace Domain.Pipelines;

internal sealed class StepName : ValueObject
{
    public string Value { get; private set; } = string.Empty;

    public StepName(string value)
    {
        AddNotifications(
            new Contract<StepName>()
                .IsNotNullOrEmpty(value, nameof(StepName), "Step name cannot be empty")
                .IsLowerOrEqualsThan(value?.Length ?? 0, 200, nameof(StepName),
                    "Step name cannot exceed 200 characters"));

        if (IsValid)
            Value = value!;
    }

    public override string ToString() => Value;
}
