using Flunt.Notifications;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ValueObjects;

internal abstract class BaseValueObject : Notifiable<Notification>
{
    [NotMapped]
    public new IReadOnlyCollection<Notification> Notifications => base.Notifications;
}