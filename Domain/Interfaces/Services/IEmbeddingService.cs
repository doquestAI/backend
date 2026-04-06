namespace Domain.Interfaces.Services;

internal interface IEmbeddingService
{
    Task<float[]> GenerateAsync(string text, CancellationToken cancellationToken = default);
}
