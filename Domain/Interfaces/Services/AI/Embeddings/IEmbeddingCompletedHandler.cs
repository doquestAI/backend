using Domain.Messages;

namespace Domain.Interfaces.Services.AI.Embeddings;

internal interface IEmbeddingCompletedService
{
    Task ExecuteAsync(EmbeddingCompletedMessage message, CancellationToken cancellationToken);
}