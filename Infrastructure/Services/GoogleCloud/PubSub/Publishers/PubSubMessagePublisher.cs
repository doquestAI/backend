using Domain.Interfaces.Services;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Infrastructure.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Infrastructure.Services.GoogleCloud.PubSub.Publishers;

internal sealed partial class PubSubMessagePublisher(
    IOptions<PubSubSettings> settings,
    ILogger<PubSubMessagePublisher> logger) : IMessagePublisher
{
    private readonly PubSubSettings _settings = settings.Value;
    private readonly Dictionary<string, PublisherClient> _publishers = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task PublishAsync<TMessage>(
        string topicName,
        TMessage message,
        CancellationToken cancellationToken) where TMessage : class
    {
        var publisher = await GetOrCreatePublisher(topicName);
        var jsonMessage = JsonSerializer.Serialize(message);
        var pubsubMessage = new PubsubMessage
        {
            Data = ByteString.CopyFromUtf8(jsonMessage),
            Attributes =
            {
                ["messageType"] = typeof(TMessage).Name,
                ["timestamp"] = DateTime.UtcNow.ToString("O")
            }
        };

        var messageId = await publisher.PublishAsync(pubsubMessage);
        LogMessagePublishedToTopicTopicnameWithIdMessageid(logger, topicName, messageId);
    }

    public async Task PublishAsync<TMessage>(
        TMessage message,
        CancellationToken cancellationToken) where TMessage : class
    {
        var topicName = ResolveTopicName<TMessage>();
        await PublishAsync(topicName, message, cancellationToken);
    }

    private string ResolveTopicName<TMessage>() where TMessage : class
    {
        var messageTypeName = typeof(TMessage).Name;

        return messageTypeName switch
        {
            nameof(Domain.Messages.NotificationEmailMessage) => _settings.Topics.EmailNotification,
            _ => throw new ArgumentException($"Unknown message type: {messageTypeName}")
        };
    }

    private async Task<PublisherClient> GetOrCreatePublisher(string topicName)
    {
        await _lock.WaitAsync();
        try
        {
            if (_publishers.TryGetValue(topicName, out var existingPublisher))
            {
                return existingPublisher;
            }

            var topicPath = TopicName.FromProjectTopic(_settings.ProjectId, topicName);
            var publisher = await PublisherClient.CreateAsync(topicPath);
            _publishers[topicName] = publisher;
            return publisher;
        }
        finally
        {
            _lock.Release();
        }
    }

    [LoggerMessage(LogLevel.Information, "Message published to topic {topicName} with ID {messageId}")]
    static partial void LogMessagePublishedToTopicTopicnameWithIdMessageid(ILogger<PubSubMessagePublisher> logger, string topicName, string messageId);
}