using Domain.Configurations;
using Domain.Interfaces.Handlers;
using Domain.Messages;
using Infrastructure.Services.Azure.ServiceBus.Subscribers.Base;
using Infrastructure.Services.Azure.ServiceBus.Subscribers.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Azure.ServiceBus.Subscribers;

internal sealed class FileUploadSubscriber(
    IServiceScopeFactory scopeFactory,
    ServiceBusProcessorFactory factory,
    IOptions<ServiceBusSettings> options,
    ILogger<FileUploadSubscriber> logger)
    : ServiceBusSubscriberBase<StorageUploadMessage, IStorageUploadHandler>(
        scopeFactory,
        factory.Create(options.Value.Queues.FileUpload),
        logger)
{
    protected override Task HandleAsync(
        IStorageUploadHandler handler,
        StorageUploadMessage message,
        CancellationToken cancellationToken)
        => handler.ExecuteAsync(message, cancellationToken);
}