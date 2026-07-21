using Flunt.Notifications;

namespace Domain.Shared.Core;

/// <summary>
/// Entidade de domínio.
/// Entidades têm identidade única (mesmo se os valores mudam).
/// Todas as entidades são notificáveis (para validações sem exceções).
/// </summary>
public abstract class Entity : Notifiable<Notification>
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; private set; }

    protected Entity() { }

    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
        {
            AddNotification(nameof(Id), "Entity ID cannot be empty");
            return;
        }
        Id = id;
    }

    /// <summary>Marca entidade como deletada (soft delete).</summary>
    public void SoftDelete() => DeletedAt = DateTime.UtcNow;

    /// <summary>Recupera entidade deletada.</summary>
    public void Restore() => DeletedAt = null;

    /// <summary>Valida integridade de value objects filhos.</summary>
    protected void AddNotificationsFromValueObjects(params Notifiable<Notification>?[] valueObjects)
    {
        foreach (var vo in valueObjects.Where(v => v != null))
            AddNotifications(vo!.Notifications);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Entity? left, Entity? right) => !(left == right);
}
