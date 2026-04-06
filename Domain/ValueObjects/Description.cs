using Flunt.Validations;

namespace Domain.ValueObjects;

internal class Description : BaseValueObject
{
    public string Value { get; private set; } = string.Empty;

    protected Description() { }

    public Description(string value)
    {
        AddNotifications(new Contract<Description>()
            .Requires()
            .IsLowerOrEqualsThan(value?.Length ?? 0, 1000, nameof(Value), "Descrição deve ter no máximo 1000 caracteres"));

        if (IsValid)
            Value = value ?? string.Empty;
    }

    public override string ToString() => Value;
}
