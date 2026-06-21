using Domain.Common.Responses;
using Domain.Configurations;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Messages;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.UseCases.Commands.User.Activate;

internal class Handler(
    IUserRepository userRepository,
    IDbCommit dbCommit,
    IMessagePublisher messagePublisher,
    IOptions<SmtpSettings> smtpSettings,
    IOptions<PubSubSettings> pubSubSettings
) : IRequestHandler<Request, BaseResponse>
{
    public async Task<BaseResponse> Handle(Request request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.token))
            return new BaseResponse(400, "Token inválido");

        var user = await userRepository.ActivateUserAsync(request.email, request.token, cancellationToken);
        if (user is null)
            return new BaseResponse(400, "Usuário ou token de ativação inválido");

        user.SetPassword(new Domain.ValueObjects.Password(request.password));
        if (user.Notifications.Any())
            return new BaseResponse(400, "Dados inválidos", user.Notifications.ToList());

        var emailMessage = new NotificationEmailMessage(
            RecipientEmail: user.Email.Address!,
            RecipientName: user.FullName.FirstName!,
            Subject: "Parabéns, Agora você é Team Kdesk!",
            Content: $"<strong> Sua conta foi ativada com sucesso. Bem-vindo ao Kdesk! </strong>",
            IsHtmlContent: true,
            SenderName: "Kdesk Team",
            SenderEmail: smtpSettings.Value.User
        );

        var commit = dbCommit.Commit(cancellationToken);
        var emailTask = messagePublisher.PublishAsync(pubSubSettings.Value.Topics.EmailNotification, emailMessage, cancellationToken);
        await Task.WhenAll(commit, emailTask);
        return new BaseResponse(200, "Usuário ativado com sucesso!");
    }
}