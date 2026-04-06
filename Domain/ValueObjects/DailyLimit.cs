using Flunt.Validations;

namespace Domain.ValueObjects;

internal class DailyLimit : BaseValueObject
{
    public int Value { get; private set; }

    protected DailyLimit() { }

    public DailyLimit(int value, string fieldName)
    {
        AddNotifications(new Contract<DailyLimit>()
            .Requires()
            .IsGreaterOrEqualsThan(value, -1, fieldName, $"Limite de {fieldName} inválido (mínimo -1, onde -1 = ilimitado)"));

        if (IsValid)
            Value = value;
    }

    public bool IsUnlimited => Value == -1;

    public bool IsWithinLimit(int currentCount) =>
        IsUnlimited || currentCount < Value;

    public override string ToString() => IsUnlimited ? "Ilimitado" : Value.ToString();
}
