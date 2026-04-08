using Domain.Interfaces.Services.AI.Embeddings;
using Domain.Messages;
using Infrastructure.Configurations;
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
        IEmbeddingCompletedService>(scopeFactory,
        factory.Create(
            options.Value.Subscriptions.EmbeddingCompleted),
        logger)
{
    protected override Task HandleAsync(
        IEmbeddingCompletedService handler,
        EmbeddingCompletedMessage message,
        CancellationToken cancellationToken)
        => handler.ExecuteAsync(message, cancellationToken);
}