using Flunt.Validations;

namespace Domain.ValueObjects;

internal class DisplayName : BaseValueObject
{
    public string Value { get; private set; } = string.Empty;

    protected DisplayName() { }

    public DisplayName(string value)
    {
        AddNotifications(new Contract<DisplayName>()
            .Requires()
            .IsNotNullOrEmpty(value, nameof(Value), "DisplayName é obrigatório")
            .IsLowerOrEqualsThan(value?.Length ?? 0, 200, nameof(Value), "DisplayName deve ter no máximo 200 caracteres"));

        if (IsValid)
            Value = value!;
    }

    public override string ToString() => Value;
}
