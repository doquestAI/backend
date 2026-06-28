namespace Domain.Interfaces.Services;

internal interface IMessagePublisher
{
    Task PublishAsync<TMessage>(
        string topicName,
        TMessage message,
        CancellationToken cancellationToken) where TMessage : class;

    Task PublishAsync<TMessage>(
        TMessage message,
        CancellationToken cancellationToken) where TMessage : class;
}