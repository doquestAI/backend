using Flunt.Validations;

namespace Domain.ValueObjects;

internal class FirebaseUid : BaseValueObject
{
    public string Value { get; private set; } = string.Empty;

    protected FirebaseUid() { }

    public FirebaseUid(string value)
    {
        AddNotifications(new Contract<FirebaseUid>()
            .Requires()
            .IsNotNullOrEmpty(value, nameof(Value), "Firebase UID é obrigatório")
            .IsLowerOrEqualsThan(value?.Length ?? 0, 200, nameof(Value), "Firebase UID deve ter no máximo 200 caracteres"));

        if (IsValid)
            Value = value!;
    }

    public override string ToString() => Value;
}
