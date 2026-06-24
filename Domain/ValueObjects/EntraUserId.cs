using Flunt.Br;

namespace Domain.ValueObjects;

internal sealed class EntraUserId : BaseValueObject
{
    public string Value { get; private set; } = string.Empty;

    public EntraUserId() { }

    public EntraUserId(string? value)
    {
        AddNotifications(
            new Contract()
                .Requires()
                .IsNotNullOrEmpty(value, nameof(EntraUserId), "EntraUserId cannot be null or empty")
                .IsTrue(value != null && value.Length >= 10, nameof(EntraUserId), "EntraUserId has invalid format")
        );

        if (!IsValid)
            return;

        Value = value!;
    }
}