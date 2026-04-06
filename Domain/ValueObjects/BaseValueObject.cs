using System.ComponentModel.DataAnnotations.Schema;
using Flunt.Notifications;

namespace Domain.ValueObjects;

internal abstract class BaseValueObject : Notifiable<Notification>
{
    [NotMapped]
    public new IReadOnlyCollection<Notification> Notifications => base.Notifications;
}
