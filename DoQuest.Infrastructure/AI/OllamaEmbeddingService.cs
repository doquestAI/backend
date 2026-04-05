using DoQuest.Application.Abstractions;
using DoQuest.Infrastructure.Options;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;

namespace DoQuest.Infrastructure.AI;

public sealed class OllamaEmbeddingService : IEmbeddingService
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _generator;

    public OllamaEmbeddingService(IOptions<OllamaOptions> options)
    {
        var opts = options.Value;
        _generator = new OllamaApiClient(new Uri(opts.Endpoint))
            .AsEmbeddingGenerator(opts.EmbeddingModel);
    }

    public async Task<float[]> GenerateAsync(string text, CancellationToken cancellationToken = default)
    {
        var result = await _generator.GenerateAsync(text, cancellationToken: cancellationToken);
        return result.Vector.ToArray();
    }
}
