
using Infrastructure.Configurations;
using Domain.Interfaces.Handlers;
using Domain.Messages;
using Infrastructure.Services.GoogleCloud.PubSub.Subscribers.Base;
using Infrastructure.Services.GoogleCloud.PubSub.Subscribers.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.GoogleCloud.PubSub.Subscribers;

internal sealed class EmailSenderSubscriber(
    IServiceScopeFactory scopeFactory,
    SubscriberFactory factory,
    IOptions<PubSubSettings> options,
    ILogger<EmailSenderSubscriber> logger)
    : SubscriberBase<
        NotificationEmailMessage,
        IEmailNotificationHandler>(scopeFactory,
        factory.Create(
            options.Value.Subscriptions.EmailNotification),
        logger)
{
    protected override Task HandleAsync(
        IEmailNotificationHandler handler,
        NotificationEmailMessage message,
        CancellationToken cancellationToken)
        => handler.ExecuteAsync(message, cancellationToken);
}