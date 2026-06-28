using Flunt.Notifications;

namespace AI.Exceptions;

/// <summary>
/// Única exception permitida pela arquitetura — encapsula Domain Notifications
/// para cruzar a fronteira de camada quando absolutamente necessário (ex: precondição
/// de DI violada). Fluxos normais devem usar <c>PipelineResult</c>.
/// </summary>
public sealed class DomainNotificationException : Exception
{
    public IReadOnlyCollection<Notification> Notifications { get; }

    public DomainNotificationException(IReadOnlyCollection<Notification> notifications)
        : base("One or more domain notifications occurred: "
               + string.Join("; ", notifications.Select(n => $"{n.Key}: {n.Message}")))
    {
        Notifications = notifications;
    }
}
