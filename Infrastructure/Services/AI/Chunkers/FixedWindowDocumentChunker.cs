using Domain.Constants;
using Domain.Interfaces.Services.Chunkers;

namespace Infrastructure.Services.AI.Chunkers;


internal sealed class FixedWindowDocumentChunkerService : IDocumentChunkerService
{
    public IReadOnlyList<string> Chunk(string rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
            return [];

        var words = rawText.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (words.Length == 0)
            return [];

        var chunks = new List<string>();
        var windowSize = ModelConstants.MaxChunkTokens;
        var overlap = ModelConstants.ChunkOverlapTokens;
        var step = windowSize - overlap;

        for (var i = 0; i < words.Length; i += step)
        {
            var end = Math.Min(i + windowSize, words.Length);
            var chunk = string.Join(' ', words[i..end]);

            if (!string.IsNullOrWhiteSpace(chunk))
                chunks.Add(chunk);

            if (end >= words.Length)
                break;
        }

        return chunks.AsReadOnly();
    }
}