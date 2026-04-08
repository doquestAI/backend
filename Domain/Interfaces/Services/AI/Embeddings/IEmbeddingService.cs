namespace Domain.Interfaces.Services.AI.Embeddings;

internal interface IEmbeddingService
{
    Task<float[]> GenerateAsync(string text, CancellationToken cancellationToken = default);
}