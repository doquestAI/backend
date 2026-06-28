using Domain.Common;
using Flunt.Validations;

namespace Domain.Capabilities.ValueObjects;

public sealed class PluginName : ValueObject
{
    public string Value { get; private set; } = string.Empty;

    public PluginName(string value)
    {
        AddNotifications(
            new Contract<PluginName>()
                .IsNotNullOrEmpty(value, nameof(PluginName), "Plugin name cannot be empty")
                .IsLowerOrEqualsThan(value?.Length ?? 0, 100, nameof(PluginName),
                    "Plugin name cannot exceed 100 characters"));

        if (IsValid)
            Value = value!;
    }

    public override string ToString() => Value;
}
