using Flunt.Notifications;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ValueObjects;

internal abstract class BaseValueObject : Notifiable<Notification>
{
    protected string Key { get; }
    [NotMapped]
    public IReadOnlyCollection<Notification> Notifications => base.Notifications;
    protected BaseValueObject()
    {
        Key = GetType().Name;
    }
}