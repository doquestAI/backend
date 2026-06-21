using Domain.Configurations;
using Domain.Interfaces.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Services;

internal sealed class EmailService(
    IOptions<SmtpSettings> smtpSettings
) : IEmailService
{
    public async Task SendEmailAsync(string toName, string toEmail, string subject, string body, string fromName,
        string fromEmail, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
            throw new ArgumentException("O endereço de email do destinatário não pode ser vazio.", nameof(toEmail));

        if (string.IsNullOrWhiteSpace(fromEmail))
            throw new ArgumentException("O endereço de email do remetente não pode ser vazio.", nameof(fromEmail));

        var smtp = new SmtpClient(smtpSettings.Value.Server, smtpSettings.Value.Port)
        {
            Credentials = new NetworkCredential(smtpSettings.Value.User, smtpSettings.Value.Pass),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            EnableSsl = true
        };

        var mail = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mail.To.Add(new MailAddress(toEmail, toName));
        await smtp.SendMailAsync(mail, cancellationToken);
    }
}