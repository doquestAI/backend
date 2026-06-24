using Azure.Messaging.ServiceBus;
using Domain.Configurations;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Azure.ServiceBus.Subscribers.Factories;

internal sealed class ServiceBusProcessorFactory(
    ServiceBusClient client,
    IOptions<ServiceBusSettings> options)
{
    private readonly ServiceBusSettings _settings = options.Value;

    public ServiceBusProcessor Create(string queueName) =>
        client.CreateProcessor(queueName, new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = _settings.Queues.MaxConcurrentMessages,
            AutoCompleteMessages = false
        });
}
