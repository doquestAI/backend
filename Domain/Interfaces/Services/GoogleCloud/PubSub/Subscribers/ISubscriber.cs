namespace Domain.Interfaces.Subscribers;

internal interface ISubscriber<TMessage>
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}