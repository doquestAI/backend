using Domain.Interfaces.Services.Cloud.Storage;
using Domain.Messages;
using Infrastructure.Configurations;
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
        IStorageUploadService>(scopeFactory,
        factory.Create(
            options.Value.Subscriptions.FileUpload),
        logger)
{
    protected override Task HandleAsync(
        IStorageUploadService handler,
        StorageUploadMessage message,
        CancellationToken cancellationToken)
        => handler.ExecuteAsync(message, cancellationToken);
}