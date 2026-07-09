namespace Domain.Interfaces.Services;

internal interface IEmbeddingService
{
    Task<ReadOnlyMemory<float>> GenerateAsync(string text, CancellationToken cancellationToken = default);
    Task<List<ReadOnlyMemory<float>>> GenerateBatchAsync(List<string> texts, CancellationToken cancellationToken = default);
}
