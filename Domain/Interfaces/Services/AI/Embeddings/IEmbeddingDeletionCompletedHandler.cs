using Domain.Messages;

namespace Domain.Interfaces.Services.AI.Embeddings;

internal interface IEmbeddingDeletionCompletedService
{
    Task ExecuteAsync(EmbeddingDeletionCompletedMessage message, CancellationToken cancellationToken);
}