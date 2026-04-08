namespace Domain.Interfaces.Services.Email;

internal interface IEmailService
{
    Task SendEmailAsync(string toName, string toEmail, string subject,
         string body, string fromName, string fromEmail, CancellationToken cancellationToken);
}