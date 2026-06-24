using Azure.Messaging.ServiceBus;
using Domain.Configurations;
using Domain.Interfaces.Services;
using Domain.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Infrastructure.Services.Azure.ServiceBus.Publishers;

internal sealed partial class ServiceBusMessagePublisher(
    ServiceBusClient client,
    IOptions<ServiceBusSettings> settings,
    ILogger<ServiceBusMessagePublisher> logger) : IMessagePublisher
{
    private readonly ServiceBusSettings _settings = settings.Value;
    private readonly Dictionary<string, ServiceBusSender> _senders = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task PublishAsync<TMessage>(
        string queueName,
        TMessage message,
        CancellationToken cancellationToken) where TMessage : class
    {
        var sender = await GetOrCreateSender(queueName);
        var json = JsonSerializer.Serialize(message);
        var sbMessage = new ServiceBusMessage(json)
        {
            ApplicationProperties =
            {
                ["messageType"] = typeof(TMessage).Name,
                ["timestamp"] = DateTime.UtcNow.ToString("O")
            }
        };

        await sender.SendMessageAsync(sbMessage, cancellationToken);
        LogMessagePublished(logger, queueName, sbMessage.MessageId);
    }

    public async Task PublishAsync<TMessage>(
        TMessage message,
        CancellationToken cancellationToken) where TMessage : class
    {
        var queueName = ResolveQueueName<TMessage>();
        await PublishAsync(queueName, message, cancellationToken);
    }

    private string ResolveQueueName<TMessage>() where TMessage : class =>
        typeof(TMessage).Name switch
        {
            nameof(NotificationEmailMessage) => _settings.Queues.EmailNotification,
            _ => throw new ArgumentException($"Unknown message type: {typeof(TMessage).Name}")
        };

    private async Task<ServiceBusSender> GetOrCreateSender(string queueName)
    {
        await _lock.WaitAsync();
        try
        {
            if (_senders.TryGetValue(queueName, out var existing))
                return existing;

            var sender = client.CreateSender(queueName);
            _senders[queueName] = sender;
            return sender;
        }
        finally
        {
            _lock.Release();
        }
    }

    [LoggerMessage(LogLevel.Information, "Message published to queue {queueName} with ID {messageId}")]
    static partial void LogMessagePublished(ILogger<ServiceBusMessagePublisher> logger, string queueName, string messageId);
}
