using Domain.Interfaces.Services;
using Microsoft.Extensions.AI;

namespace Infrastructure.Services;

internal class EmbeddingService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator) : IEmbeddingService
{
    public async Task<ReadOnlyMemory<float>> GenerateAsync(string text, CancellationToken cancellationToken = default)
    {
        var embedding = await embeddingGenerator.GenerateAsync([text], null, cancellationToken);
        return embedding.FirstOrDefault().Vector;
    }

    public async Task<List<ReadOnlyMemory<float>>> GenerateBatchAsync(List<string> texts, CancellationToken cancellationToken = default)
    {
        var embeddings = await embeddingGenerator.GenerateAsync(texts, null, cancellationToken);
        return embeddings.Select(e => e.Vector).ToList();
    }
}
