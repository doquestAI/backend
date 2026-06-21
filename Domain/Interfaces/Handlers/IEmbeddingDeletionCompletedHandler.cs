using Domain.Messages;

namespace Domain.Interfaces.Handlers;

public interface IEmbeddingDeletionCompletedHandler
{
    Task ExecuteAsync(EmbeddingDeletionCompletedMessage message, CancellationToken cancellationToken);
}