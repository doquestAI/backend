using Domain.Messages;

namespace Domain.Interfaces.Handlers;

internal interface IEmailNotificationHandler
{
    Task ExecuteAsync(NotificationEmailMessage message, CancellationToken cancellationToken);
}