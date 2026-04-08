using Domain.Interfaces.Services.AI.Embeddings;
using Infrastructure.Options;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;

namespace Infrastructure.AI;

internal sealed class OllamaEmbeddingService : IEmbeddingService
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _generator;

    public OllamaEmbeddingService(IOptions<OllamaOptions> options)
    {
        var opts = options.Value;
        _generator = new OllamaApiClient(new Uri(opts.Endpoint), opts.EmbeddingModel);
    }

    public async Task<float[]> GenerateAsync(string text, CancellationToken cancellationToken = default)
    {
        var result = await _generator.GenerateAsync(text, cancellationToken: cancellationToken);
        return result.Vector.ToArray();
    }
}