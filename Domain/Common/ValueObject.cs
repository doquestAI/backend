using Flunt.Notifications;

namespace Domain.Common;

/// <summary>
/// Base pública para Value Objects do Domain de Agents.
///
/// Acumula notificações via Flunt em vez de lançar exceptions —
/// toda falha de regra de negócio vira <see cref="Notification"/>.
///
/// Coexiste com <see cref="ValueObjects.BaseValueObject"/> (internal) que continua
/// servindo aos VOs legados (Email, Cpf, etc.). Esta base existe porque a
/// camada AI precisa herdar/instanciar VOs do Domain.
/// </summary>
internal abstract class ValueObject : Notifiable<Notification>;
