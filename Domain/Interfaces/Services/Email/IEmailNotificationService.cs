using Domain.Messages;

namespace Domain.Interfaces.Services.Email;

internal interface IEmailNotificationService
{
    Task ExecuteAsync(NotificationEmailMessage message, CancellationToken cancellationToken);
}