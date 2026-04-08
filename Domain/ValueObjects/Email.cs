using Flunt.Validations;

namespace Domain.ValueObjects;

internal class Email : BaseValueObject
{
    public string Address { get; private set; } = string.Empty;

    protected Email() { }

    public Email(string address)
    {
        AddNotifications(new Contract<Email>()
            .Requires()
            .IsEmail(address, nameof(Address), "Email inválido")
            .IsLowerOrEqualsThan(address?.Length ?? 0, 300, nameof(Address), "Email deve ter no máximo 300 caracteres"));

        if (IsValid)
            Address = address!;
    }

    public override string ToString() => Address;
}