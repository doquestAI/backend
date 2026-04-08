using Flunt.Validations;

namespace Domain.ValueObjects;

internal class ContextLength : BaseValueObject
{
    public int Value { get; private set; }

    protected ContextLength() { }

    public ContextLength(int value)
    {
        AddNotifications(new Contract<ContextLength>()
            .Requires()
            .IsGreaterThan(value, 0, nameof(Value), "ContextLength deve ser maior que 0"));

        if (IsValid)
            Value = value;
    }

    public override string ToString() => Value.ToString();
}