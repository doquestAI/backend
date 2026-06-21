using Flunt.Notifications;

namespace Domain.Entities.Abstracts;

internal abstract class Entity : Notifiable<Notification>
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime? CreatedDate { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; private set; } = DateTime.UtcNow;
    public DateTime? DeletedDate { get; private set; }

    protected void AddNotificationsFromValueObjects(params Notifiable<Notification>?[] valueObjects)
    {
        foreach (var valueObject in valueObjects)
        {
            if (valueObject != null && valueObject.Notifications != null)
                AddNotifications(valueObject.Notifications);
        }
    }
    public void SetValuesDelete() => DeletedDate = DateTime.UtcNow;
}