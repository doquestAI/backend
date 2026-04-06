using Flunt.Validations;

namespace Domain.ValueObjects;

internal class VestibularName : BaseValueObject
{
    public string Value { get; private set; } = string.Empty;

    protected VestibularName() { }

    public VestibularName(string value)
    {
        AddNotifications(new Contract<VestibularName>()
            .Requires()
            .IsNotNullOrEmpty(value, nameof(Value), "Nome do vestibular é obrigatório")
            .IsLowerOrEqualsThan(value?.Length ?? 0, 200, nameof(Value), "Nome do vestibular deve ter no máximo 200 caracteres"));

        if (IsValid)
            Value = value!;
    }

    public override string ToString() => Value;
}
