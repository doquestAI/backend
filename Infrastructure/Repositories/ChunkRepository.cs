using Domain.Entities.Core.Documents;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal class ChunkRepository(CoreDbContext context)
    : BaseRepository<Chunk>(context), IChunkRepository
{
    public async Task<List<Chunk>> GetByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken)
    {
        return await context.Set<Chunk>()
            .AsNoTracking()
            .Where(c => c.DocumentId == documentId && c.DeletedDate == null)
            .OrderBy(c => c.PositionIndex)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<(Chunk chunk, float score)>> SearchSimilarAsync(
        ReadOnlyMemory<float> embedding,
        int topK = 10,
        float minScore = 0.0f,
        CancellationToken cancellationToken = default)
    {
        var queryArray = embedding.ToArray();
        var queryNorm = MathF.Sqrt(queryArray.Sum(x => x * x));

        var chunks = await context.Set<Chunk>()
            .AsNoTracking()
            .Where(c => c.DeletedDate == null && c.Embedding != null)
            .ToListAsync(cancellationToken);

        return chunks
            .Select(c => {
                var chunkMemory = c.GetEmbedding();
                if (chunkMemory.IsEmpty) return (c, Score: 0f);
                var chunkArray = chunkMemory.Span.ToArray();
                var chunkNorm = MathF.Sqrt(chunkArray.Sum(x => x * x));
                if (chunkNorm == 0 || queryNorm == 0) return (c, Score: 0f);
                var dotProduct = queryArray.Zip(chunkArray, (q, ch) => q * ch).Sum();
                var similarity = dotProduct / (queryNorm * chunkNorm);
                return (c, Score: similarity);
            })
            .Where(r => r.Score >= minScore)
            .OrderByDescending(r => r.Score)
            .Take(topK)
            .ToList();
    }

    public async Task CreateBatchAsync(List<Chunk> chunks, CancellationToken cancellationToken)
    {
        await context.Set<Chunk>().AddRangeAsync(chunks, cancellationToken);
    }
}
