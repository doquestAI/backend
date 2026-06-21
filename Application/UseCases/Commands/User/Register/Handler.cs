using AutoMapper;
using Domain.Configurations;
using Domain.Interfaces.Context;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Messages;
using Domain.ValueObjects;
using Flunt.Notifications;
using Flunt.Validations;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.UseCases.Commands.User.Register;

internal class Handler(
    IUserRepository userRepository,
    IDbCommit dbCommit,
    IMapper mapper,
    IMessagePublisher messagePublisher,
    IOptions<SmtpSettings> smtpSettings,
    IOptions<AppSettings> appSettings,
    IOptions<PubSubSettings> pubSubSettings,
    IUserContext userContext) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var user = mapper.Map<Domain.Entities.Core.User>(request, opts =>
        {
            opts.Items["UserContext"] = userContext;
        });

        user.AddNotifications(
            new Contract<Notifiable<Notification>>()
                .Requires()
                .IsFalse(await userRepository.GetByEmail(request.Email, cancellationToken) != null, "Email", "Email already registered")
        );

        var activationTokenKey = ActivationToken.Create(appSettings.Value.EncryptionKey);
        user.SetActivationToken(activationTokenKey);

        if (user.Notifications.Any())
            return new Response(404, "Request invalid", user.Notifications.ToList());

        var activationLink = @$"{appSettings.Value.PublicUrlFrontEnd}/activate-account/?token={Uri.EscapeDataString(activationTokenKey.EncryptedValue)}&email={Uri.EscapeDataString(user.Email.Address!)}";

        var emailMessage = new NotificationEmailMessage(
            RecipientEmail: user.Email.Address!,
            RecipientName: user.FullName.FirstName!,
            Subject: "Ative sua Conta!",
            Content: $"<strong> Clique no link para ativar sua conta: <a href='{activationLink}'>Ativar Conta</a> </strong>",
            IsHtmlContent: true,
            SenderName: "Kdesk Team",
            SenderEmail: smtpSettings.Value.User
        );

        var createTask = userRepository.CreateAsync(user, cancellationToken);
        var emailTask = messagePublisher.PublishAsync(pubSubSettings.Value.Topics.EmailNotification, emailMessage, cancellationToken);

        await Task.WhenAll(createTask, emailTask);
        await dbCommit.Commit(cancellationToken);

        return mapper.Map<Response>(user);
    }
}