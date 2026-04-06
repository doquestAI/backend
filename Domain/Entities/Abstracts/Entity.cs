using Domain.ValueObjects;
using Flunt.Notifications;

namespace Domain.Entities.Abstracts;

internal abstract class Entity : Notifiable<Notification>
{
    public bool IsInvalid => !IsValid;

    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    protected void AddNotificationsFromValueObjects(params BaseValueObject?[] valueObjects)
    {
        foreach (var vo in valueObjects)
        {
            if (vo is { Notifications: not null })
                AddNotifications(vo.Notifications);
        }
    }

    public void MarkAsUpdated() => UpdatedAt = DateTime.UtcNow;

    public void MarkAsDeleted() => DeletedAt = DateTime.UtcNow;
}
