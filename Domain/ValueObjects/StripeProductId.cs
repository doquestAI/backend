using Flunt.Validations;

namespace Domain.ValueObjects;

internal class StripeProductId : BaseValueObject
{
    public string Value { get; private set; } = string.Empty;

    protected StripeProductId() { }

    public StripeProductId(string value)
    {
        AddNotifications(new Contract<StripeProductId>()
            .Requires()
            .IsNotNullOrEmpty(value, nameof(Value), "Stripe Product ID é obrigatório")
            .IsLowerOrEqualsThan(value?.Length ?? 0, 200, nameof(Value), "Stripe Product ID deve ter no máximo 200 caracteres"));

        if (IsValid)
            Value = value!;
    }

    public override string ToString() => Value;
}
