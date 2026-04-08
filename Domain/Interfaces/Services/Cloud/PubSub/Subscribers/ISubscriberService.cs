namespace Domain.Interfaces.Services.Cloud.PubSub.Subscribers;

internal interface ISubscriberService<TMessage>
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}