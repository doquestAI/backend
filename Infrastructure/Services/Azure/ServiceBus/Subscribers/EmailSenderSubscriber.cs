using Domain.Configurations;
using Domain.Interfaces.Handlers;
using Domain.Messages;
using Infrastructure.Services.Azure.ServiceBus.Subscribers.Base;
using Infrastructure.Services.Azure.ServiceBus.Subscribers.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Azure.ServiceBus.Subscribers;

internal sealed class EmailSenderSubscriber(
    IServiceScopeFactory scopeFactory,
    ServiceBusProcessorFactory factory,
    IOptions<ServiceBusSettings> options,
    ILogger<EmailSenderSubscriber> logger)
    : ServiceBusSubscriberBase<NotificationEmailMessage, IEmailNotificationHandler>(
        scopeFactory,
        factory.Create(options.Value.Queues.EmailNotification),
        logger)
{
    protected override Task HandleAsync(
        IEmailNotificationHandler handler,
        NotificationEmailMessage message,
        CancellationToken cancellationToken)
        => handler.ExecuteAsync(message, cancellationToken);
}