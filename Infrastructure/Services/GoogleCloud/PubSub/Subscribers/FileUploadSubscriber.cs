using Infrastructure.Configurations;
using Domain.Interfaces.Handlers;
using Domain.Messages;
using Infrastructure.Services.GoogleCloud.PubSub.Subscribers.Base;
using Infrastructure.Services.GoogleCloud.PubSub.Subscribers.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.GoogleCloud.PubSub.Subscribers;

internal sealed class FileUploadSubscriber(
    IServiceScopeFactory scopeFactory,
    SubscriberFactory factory,
    IOptions<PubSubSettings> options,
    ILogger<FileUploadSubscriber> logger)
    : SubscriberBase<
        StorageUploadMessage,
        IStorageUploadHandler>(scopeFactory,
        factory.Create(
            options.Value.Subscriptions.FileUpload),
        logger)
{
    protected override Task HandleAsync(
        IStorageUploadHandler handler,
        StorageUploadMessage message,
        CancellationToken cancellationToken)
        => handler.ExecuteAsync(message, cancellationToken);
}