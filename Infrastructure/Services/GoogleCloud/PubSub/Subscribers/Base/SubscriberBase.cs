using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services.GoogleCloud.PubSub.Subscribers.Base;

internal abstract partial class SubscriberBase<TMessage, THandler>(
    IServiceScopeFactory scopeFactory,
    SubscriberClient subscriber,
    ILogger<SubscriberBase<TMessage, THandler>> logger)
    : BackgroundService
    where THandler : class
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogSubscriberStarting(logger, GetType().Name);
        await subscriber.StartAsync(async (message, token) =>
        {
            try
            {
                var payload = JsonSerializer.Deserialize<TMessage>(
                    message.Data.ToStringUtf8());

                if (payload is null)
                    return SubscriberClient.Reply.Nack;

                using var scope = scopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<THandler>();

                await HandleAsync(handler, payload, token);

                return SubscriberClient.Reply.Ack;
            }
            catch (Exception ex)
            {
                LogSubscriberErrorProcessingMessagetype(logger, ex, GetType().Name, typeof(TMessage).Name);

                return SubscriberClient.Reply.Nack;
            }
        });
    }

    protected abstract Task HandleAsync(
        THandler handler,
        TMessage message,
        CancellationToken cancellationToken);

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        LogSubscriberStopping(logger, GetType().Name);

        await subscriber.StopAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    [LoggerMessage(LogLevel.Information, "{subscriber} starting...")]
    static partial void LogSubscriberStarting(ILogger<SubscriberBase<TMessage, THandler>> logger, string subscriber);

    [LoggerMessage(LogLevel.Error, "{subscriber} error processing {messageType}")]
    static partial void LogSubscriberErrorProcessingMessagetype(
        ILogger<SubscriberBase<TMessage, THandler>> logger, Exception exception,
        string subscriber, string messageType);

    [LoggerMessage(LogLevel.Information, "{subscriber} stopping...")]
    static partial void LogSubscriberStopping(ILogger<SubscriberBase<TMessage, THandler>> logger, string subscriber);
}