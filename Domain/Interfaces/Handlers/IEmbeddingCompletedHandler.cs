using Domain.Messages;

namespace Domain.Interfaces.Handlers;

internal interface IEmbeddingCompletedHandler
{
    Task ExecuteAsync(EmbeddingCompletedMessage message, CancellationToken cancellationToken);
}