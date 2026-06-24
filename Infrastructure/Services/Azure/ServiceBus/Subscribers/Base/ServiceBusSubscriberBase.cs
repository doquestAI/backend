using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services.Azure.ServiceBus.Subscribers.Base;

internal abstract partial class ServiceBusSubscriberBase<TMessage, THandler>(
    IServiceScopeFactory scopeFactory,
    ServiceBusProcessor processor,
    ILogger<ServiceBusSubscriberBase<TMessage, THandler>> logger)
    : BackgroundService
    where THandler : class
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogSubscriberStarting(logger, GetType().Name);
        processor.ProcessMessageAsync += OnMessageReceivedAsync;
        processor.ProcessErrorAsync += OnErrorAsync;
        await processor.StartProcessingAsync(stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
    }

    private async Task OnMessageReceivedAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<TMessage>(args.Message.Body.ToString());

            if (payload is null)
            {
                await args.DeadLetterMessageAsync(args.Message, "DeserializationFailed",
                    "Could not deserialize message body", args.CancellationToken);
                return;
            }

            using var scope = scopeFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<THandler>();
            await HandleAsync(handler, payload, args.CancellationToken);
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
        }
        catch (Exception ex)
        {
            LogErrorProcessingMessage(logger, ex, GetType().Name, typeof(TMessage).Name);
            await args.AbandonMessageAsync(args.Message, cancellationToken: args.CancellationToken);
        }
    }

    private Task OnErrorAsync(ProcessErrorEventArgs args)
    {
        LogProcessorError(logger, args.Exception, GetType().Name, args.ErrorSource.ToString());
        return Task.CompletedTask;
    }

    protected abstract Task HandleAsync(THandler handler, TMessage message, CancellationToken cancellationToken);

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        LogSubscriberStopping(logger, GetType().Name);
        await processor.StopProcessingAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    [LoggerMessage(LogLevel.Information, "{subscriber} starting...")]
    static partial void LogSubscriberStarting(ILogger<ServiceBusSubscriberBase<TMessage, THandler>> logger, string subscriber);

    [LoggerMessage(LogLevel.Error, "{subscriber} error processing {messageType}")]
    static partial void LogErrorProcessingMessage(ILogger<ServiceBusSubscriberBase<TMessage, THandler>> logger, Exception exception, string subscriber, string messageType);

    [LoggerMessage(LogLevel.Error, "{subscriber} processor error from {errorSource}")]
    static partial void LogProcessorError(ILogger<ServiceBusSubscriberBase<TMessage, THandler>> logger, Exception exception, string subscriber, string errorSource);

    [LoggerMessage(LogLevel.Information, "{subscriber} stopping...")]
    static partial void LogSubscriberStopping(ILogger<ServiceBusSubscriberBase<TMessage, THandler>> logger, string subscriber);
}
