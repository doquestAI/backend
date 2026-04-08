using Domain.Interfaces.Services.AI.Embeddings;
using Domain.Messages;
using Infrastructure.Configurations;
using Infrastructure.Services.GoogleCloud.PubSub.Subscribers.Base;
using Infrastructure.Services.GoogleCloud.PubSub.Subscribers.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.GoogleCloud.PubSub.Subscribers;

internal sealed class EmbeddingDeletionCompletedSubscriber(
    IServiceScopeFactory scopeFactory,
    SubscriberFactory factory,
    IOptions<PubSubSettings> options,
    ILogger<EmbeddingDeletionCompletedSubscriber> logger)
    : SubscriberBase<
        EmbeddingDeletionCompletedMessage,
        IEmbeddingDeletionCompletedService>(scopeFactory,
        factory.Create(
            options.Value.Subscriptions.EmbeddingDeletionCompleted),
        logger)
{
    protected override Task HandleAsync(
        IEmbeddingDeletionCompletedService handler,
        EmbeddingDeletionCompletedMessage message,
        CancellationToken cancellationToken)
        => handler.ExecuteAsync(message, cancellationToken);
}