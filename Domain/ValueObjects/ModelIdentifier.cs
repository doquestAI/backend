using Flunt.Validations;

namespace Domain.ValueObjects;

internal class ModelIdentifier : BaseValueObject
{
    public string Value { get; private set; } = string.Empty;

    protected ModelIdentifier() { }

    public ModelIdentifier(string value)
    {
        AddNotifications(new Contract<ModelIdentifier>()
            .Requires()
            .IsNotNullOrEmpty(value, nameof(Value), "ModelId é obrigatório")
            .IsLowerOrEqualsThan(value?.Length ?? 0, 200, nameof(Value), "ModelId deve ter no máximo 200 caracteres"));

        if (IsValid)
            Value = value!;
    }

    public override string ToString() => Value;
}