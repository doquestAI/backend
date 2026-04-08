using Flunt.Validations;

namespace Domain.ValueObjects;

internal class VestibularYear : BaseValueObject
{
    public int Value { get; private set; }

    protected VestibularYear() { }

    public VestibularYear(int value)
    {
        AddNotifications(new Contract<VestibularYear>()
            .Requires()
            .IsGreaterOrEqualsThan(value, 1900, nameof(Value), "Ano do vestibular deve ser >= 1900")
            .IsLowerOrEqualsThan(value, DateTime.UtcNow.Year + 1, nameof(Value), "Ano do vestibular inválido"));

        if (IsValid)
            Value = value;
    }

    public override string ToString() => Value.ToString();
}