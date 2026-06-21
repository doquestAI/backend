using Domain;
using Domain.Common.Responses;
using Domain.Configurations;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Messages;
using Flunt.Notifications;
using Flunt.Validations;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.UseCases.Commands.User.ResendCode;

internal class Handler(
    IUserRepository userRepository,
    IDbCommit dbCommit,
    IMessagePublisher messagePublisher,
    IOptions<SmtpSettings> smtpSettings,
    IOptions<PubSubSettings> pubSubSettings) : IRequestHandler<Request, BaseResponse>
{
    public async Task<BaseResponse> Handle(Request request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmail(request.email, cancellationToken);

        var contract = new Contract<Notifiable<Notification>>()
            .Requires()
            .IsNotNull(user, "Email", "Email not registered");

        user?.AddNotifications(contract);

        if (user is null || user.Notifications.Any())
        {
            return new BaseResponse(
                statusCode: 404,
                message: "Request invalid",
                notifications: user?.Notifications?.ToList()
            );
        }

        user.AssignToken(new Random().Next(1000, 9999).ToString());

        var updateTask = Task.Run(() => userRepository.Update(user), cancellationToken);
        var emailTask = messagePublisher.PublishAsync(
            pubSubSettings.Value.Topics.EmailNotification,
            new NotificationEmailMessage(
                RecipientEmail: user.Email.Address!,
                RecipientName: user.FullName.FirstName!,
                Subject: "Reenvio de Código de Ativação",
                Content: $"<strong> Seu código de Ativação da Conta: {user.TokenActivate} </strong>",
                IsHtmlContent: true,
                SenderName: "Kdesk Solutions",
                SenderEmail: smtpSettings.Value.User
            ),
            cancellationToken
        );

        await Task.WhenAll(updateTask, emailTask);
        await dbCommit.Commit(cancellationToken);

        return new BaseResponse(
            statusCode: 200,
            message: "Code sent successfully"
        );
    }
}