using Domain.Configurations;
using Domain.Interfaces.Handlers;
using Domain.Messages;
using Infrastructure.Services.Azure.ServiceBus.Subscribers.Base;
using Infrastructure.Services.Azure.ServiceBus.Subscribers.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Azure.ServiceBus.Subscribers;

internal sealed class EmbeddingCompletedSubscriber(
    IServiceScopeFactory scopeFactory,
    ServiceBusProcessorFactory factory,
    IOptions<ServiceBusSettings> options,
    ILogger<EmbeddingCompletedSubscriber> logger)
    : ServiceBusSubscriberBase<EmbeddingCompletedMessage, IEmbeddingCompletedHandler>(
        scopeFactory,
        factory.Create(options.Value.Queues.EmbeddingCompleted),
        logger)
{
    protected override Task HandleAsync(
        IEmbeddingCompletedHandler handler,
        EmbeddingCompletedMessage message,
        CancellationToken cancellationToken)
        => handler.ExecuteAsync(message, cancellationToken);
}