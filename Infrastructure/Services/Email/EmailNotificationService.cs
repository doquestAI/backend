
using Domain.Interfaces.Services.Email;
using Domain.Messages;

namespace Infrastructure.Services.Email;

internal class EmailNotificationHandler(IEmailService emailService)
    : IEmailNotificationService
{
    private readonly IEmailService _emailService = emailService;
    public Task ExecuteAsync(NotificationEmailMessage message, CancellationToken cancellationToken)
        => _emailService.SendEmailAsync(
            message.RecipientName,
            message.RecipientEmail,
            message.Subject,
            message.Content,
            message.SenderName ?? " Doquest Team",
            message.SenderEmail ?? "no-reply@doquest.com",
            cancellationToken
        );
}