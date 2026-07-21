using Flunt.Notifications;

namespace Domain.Shared.Core;

/// <summary>
/// Value Object abstrato de domínio.
/// Imutáveis, sem identidade, comparados por valor.
/// Acumula notificações Flunt para validação sem exceções.
/// </summary>
public abstract class ValueObject : Notifiable<Notification>
{
    /// <summary>Sobrescrever em subclasses para comparação de valor.</summary>
    public abstract override bool Equals(object? obj);

    /// <summary>Sobrescrever em subclasses.</summary>
    public abstract override int GetHashCode();

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);
}
