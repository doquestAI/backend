using Infrastructure.Configurations;
using Domain.Interfaces.Handlers;
using Domain.Messages;
using Infrastructure.Services.GoogleCloud.PubSub.Subscribers.Base;
using Infrastructure.Services.GoogleCloud.PubSub.Subscribers.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.GoogleCloud.PubSub.Subscribers;

internal sealed class EmbeddingCompletedSubscriber(
    IServiceScopeFactory scopeFactory,
    SubscriberFactory factory,
    IOptions<PubSubSettings> options,
    ILogger<EmbeddingCompletedSubscriber> logger)
    : SubscriberBase<
        EmbeddingCompletedMessage,
        IEmbeddingCompletedHandler>(scopeFactory,
        factory.Create(
            options.Value.Subscriptions.EmbeddingCompleted),
        logger)
{
    protected override Task HandleAsync(
        IEmbeddingCompletedHandler handler,
        EmbeddingCompletedMessage message,
        CancellationToken cancellationToken)
        => handler.ExecuteAsync(message, cancellationToken);
}