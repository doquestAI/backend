namespace DoQuest.Application.Abstractions;

public interface IEmbeddingService
{
    Task<float[]> GenerateAsync(string text, CancellationToken cancellationToken = default);
}
