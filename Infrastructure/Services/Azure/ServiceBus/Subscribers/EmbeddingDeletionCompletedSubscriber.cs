using Domain.Configurations;
using Domain.Interfaces.Handlers;
using Domain.Messages;
using Infrastructure.Services.Azure.ServiceBus.Subscribers.Base;
using Infrastructure.Services.Azure.ServiceBus.Subscribers.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Azure.ServiceBus.Subscribers;

internal sealed class EmbeddingDeletionCompletedSubscriber(
    IServiceScopeFactory scopeFactory,
    ServiceBusProcessorFactory factory,
    IOptions<ServiceBusSettings> options,
    ILogger<EmbeddingDeletionCompletedSubscriber> logger)
    : ServiceBusSubscriberBase<EmbeddingDeletionCompletedMessage, IEmbeddingDeletionCompletedHandler>(
        scopeFactory,
        factory.Create(options.Value.Queues.EmbeddingDeletionCompleted),
        logger)
{
    protected override Task HandleAsync(
        IEmbeddingDeletionCompletedHandler handler,
        EmbeddingDeletionCompletedMessage message,
        CancellationToken cancellationToken)
        => handler.ExecuteAsync(message, cancellationToken);
}