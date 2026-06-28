using Domain.Common;
using Flunt.Validations;

namespace Domain.Capabilities.ValueObjects;

internal sealed class FunctionName : ValueObject
{
    public string Value { get; private set; } = string.Empty;

    public FunctionName(string value)
    {
        AddNotifications(
            new Contract<FunctionName>()
                .IsNotNullOrEmpty(value, nameof(FunctionName), "Function name cannot be empty")
                .IsLowerOrEqualsThan(value?.Length ?? 0, 100, nameof(FunctionName),
                    "Function name cannot exceed 100 characters"));

        if (IsValid)
            Value = value!;
    }

    public override string ToString() => Value;
}
