using Domain.Interfaces.Vector;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure.Vector;

internal sealed class PgVectorStore(CoreDbContext db, ILogger<PgVectorStore> logger) : IVectorStore
{
    public async Task UpsertAsync(
        string collection,
        string id,
        string content,
        ReadOnlyMemory<float> embedding,
        IDictionary<string, string>? metadata = null,
        CancellationToken ct = default)
    {
        var vector = new Pgvector.Vector(embedding.ToArray());
        var metaJson = JsonSerializer.Serialize(metadata ?? new Dictionary<string, string>());

        var existing = await db.VectorItems
            .FirstOrDefaultAsync(v => v.Collection == collection && v.ItemId == id, ct);

        if (existing is null)
        {
            db.VectorItems.Add(new VectorItem
            {
                DbId = Guid.NewGuid(),
                ItemId = id,
                Collection = collection,
                Content = content,
                Embedding = vector,
                MetadataJson = metaJson,
                CreatedAt = DateTime.UtcNow,
            });
        }
        else
        {
            existing.Content = content;
            existing.Embedding = vector;
            existing.MetadataJson = metaJson;
        }

        await db.SaveChangesAsync(ct);

        logger.LogDebug("VectorItem upserted — Collection={Collection} Id={Id}", collection, id);
    }

    public async Task<IReadOnlyList<VectorSearchResult>> SearchAsync(
        string collection,
        ReadOnlyMemory<float> queryEmbedding,
        int topK = 5,
        float threshold = 0.75f,
        CancellationToken ct = default)
    {
        var query = new Pgvector.Vector(queryEmbedding.ToArray());
        var distanceThreshold = (double)(1f - threshold);

        var items = await db.VectorItems
            .AsNoTracking()
            .Where(v => v.Collection == collection && v.Embedding.CosineDistance(query) <= distanceThreshold)
            .OrderBy(v => v.Embedding.CosineDistance(query))
            .Take(topK)
            .ToListAsync(ct);

        return items
            .Select(v => new VectorSearchResult(
                v.ItemId,
                v.Content,
                1f - ComputeCosineDistance(v.Embedding.Memory, queryEmbedding),
                ParseMetadata(v.MetadataJson)))
            .ToList();
    }

    private static float ComputeCosineDistance(ReadOnlyMemory<float> a, ReadOnlyMemory<float> b)
    {
        var spanA = a.Span;
        var spanB = b.Span;
        float dot = 0f, normA = 0f, normB = 0f;
        for (int i = 0; i < spanA.Length; i++)
        {
            dot += spanA[i] * spanB[i];
            normA += spanA[i] * spanA[i];
            normB += spanB[i] * spanB[i];
        }
        return 1f - dot / (MathF.Sqrt(normA) * MathF.Sqrt(normB));
    }

    private static IReadOnlyDictionary<string, string> ParseMetadata(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                ?? new Dictionary<string, string>();
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }
}
