using Domain.Interfaces.Handlers;
using Domain.Interfaces.Services;
using Domain.Messages;

namespace Application.UseCases.Commands.EmailNotification;

internal class EmailNotificationHandler(IEmailService emailService)
    : IEmailNotificationHandler
{
    private readonly IEmailService _emailService = emailService;
    public Task ExecuteAsync(NotificationEmailMessage message, CancellationToken cancellationToken)
        => _emailService.SendEmailAsync(
            message.RecipientName,
            message.RecipientEmail,
            message.Subject,
            message.Content,
            message.SenderName ?? "Kdesk Team",
            message.SenderEmail ?? "no-reply@Kdesk.com",
            cancellationToken
        );
}