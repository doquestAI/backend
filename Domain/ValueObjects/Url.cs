using Flunt.Validations;

namespace Domain.ValueObjects;

internal class Url : BaseValueObject
{
    public string Value { get; private set; } = string.Empty;

    protected Url() { }

    public Url(string value)
    {
        AddNotifications(new Contract<Url>()
            .Requires()
            .IsNotNullOrEmpty(value, nameof(Value), "URL é obrigatória")
            .IsLowerOrEqualsThan(value?.Length ?? 0, 2000, nameof(Value), "URL deve ter no máximo 2000 caracteres"));

        if (IsValid)
            Value = value!;
    }

    public override string ToString() => Value;
}