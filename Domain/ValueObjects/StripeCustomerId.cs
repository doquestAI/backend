using Flunt.Br;

namespace Domain.ValueObjects;

internal sealed class StripeCustomerId : BaseValueObject
{
    public string Value { get; private set; } = string.Empty;

    public StripeCustomerId() { }

    public StripeCustomerId(string? value)
    {
        AddNotifications(
            new Contract()
                .Requires()
                .IsNotNullOrEmpty(value, nameof(StripeCustomerId), "StripeCustomerId cannot be null or empty")
                .IsTrue(value != null && value.StartsWith("cus_"), nameof(StripeCustomerId), "StripeCustomerId must start with 'cus_'")
        );

        if (!IsValid)
            return;

        Value = value!;
    }
}
