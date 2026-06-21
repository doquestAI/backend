using Flunt.Notifications;
using Flunt.Validations;

namespace Domain.ValueObjects;

internal class Url : BaseValueObject
{
    public string Endereco { get; private set; }

    protected Url() { } // Para EF
    public Url(string endereco)
    {
        Endereco = endereco?.Trim();

        AddNotifications(new Contract<Notification>()
            .Requires()
            .IsUrl(Endereco, "Url", "Url Inválida"));

        if (!IsValid)
            Endereco = null;
    }
}