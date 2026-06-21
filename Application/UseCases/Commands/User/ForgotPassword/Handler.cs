using Domain.Common.Responses;
using Domain.Configurations;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Messages;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.UseCases.Commands.User.ForgotPassword;

internal class Handler(
    IUserRepository userRepository,
    IDbCommit dbCommit,
    IMessagePublisher messagePublisher,
    IOptions<SmtpSettings> smtpSettings,
    IOptions<AppSettings> appSettings,
    IOptions<PubSubSettings> pubSubSettings) : IRequestHandler<Request, BaseResponse>
{
    public async Task<BaseResponse> Handle(Request request, CancellationToken cancellationToken)
    {
        var userFromDb = await userRepository.GetByEmail(request.Email, cancellationToken);
        if (userFromDb is null)
        {
            return new BaseResponse(
                statusCode: 404,
                message: "Usuário não encontrado"
            );
        }

        var resetToken = ActivationToken.Create(appSettings.Value.EncryptionKey);
        userFromDb.SetActivationToken(resetToken);
        userRepository.Update(userFromDb);

        var resetLink = @$"{appSettings.Value.PublicUrlFrontEnd}/forgot-password-activate/?token={resetToken.EncryptedValue}&email={Uri.EscapeDataString(userFromDb.Email.Address!)}";

        var commitTask = dbCommit.Commit(cancellationToken);
        var emailTask = messagePublisher.PublishAsync(
            pubSubSettings.Value.Topics.EmailNotification,
            new NotificationEmailMessage(
                RecipientEmail: userFromDb.Email.Address!,
                RecipientName: userFromDb.FullName.FirstName,
                Subject: "Altere sua senha!",
                Content: $"<strong> Clique no link para alterar sua senha: <a href=\"{resetLink}\">{resetLink}</a> </strong>",
                IsHtmlContent: true,
                SenderName: "Kdesk",
                SenderEmail: smtpSettings.Value.User
            ),
            cancellationToken
        );

        await Task.WhenAll(commitTask, emailTask);

        return new BaseResponse(
            statusCode: 200,
            message: "Foi enviado um email com instruções para alteração de senha!"
        );
    }
}