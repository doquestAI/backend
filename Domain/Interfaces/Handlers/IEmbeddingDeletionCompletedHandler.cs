using Domain.Messages;

namespace Domain.Interfaces.Handlers;

internal interface IEmbeddingDeletionCompletedHandler
{
    Task ExecuteAsync(EmbeddingDeletionCompletedMessage message, CancellationToken cancellationToken);
}