using Flunt.Validations;

namespace Domain.ValueObjects;

internal class PlanName : BaseValueObject
{
    public string Value { get; private set; } = string.Empty;

    protected PlanName() { }

    public PlanName(string value)
    {
        AddNotifications(new Contract<PlanName>()
            .Requires()
            .IsNotNullOrEmpty(value, nameof(Value), "Nome do plano é obrigatório")
            .IsLowerOrEqualsThan(value?.Length ?? 0, 100, nameof(Value), "Nome do plano deve ter no máximo 100 caracteres"));

        if (IsValid)
            Value = value!;
    }

    public override string ToString() => Value;
}
