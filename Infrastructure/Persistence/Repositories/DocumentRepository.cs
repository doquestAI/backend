using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Records;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal class DocumentRepository(CoreDbContext context)
    : BaseRepository<Document>(context), IDocumentRepository
{
    public async Task<Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Set<Document>()
            .FirstOrDefaultAsync(d => d.Id == id && d.DeletedAt == null, cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> GetByVestibularIdAsync(
        Guid vestibularId, CancellationToken cancellationToken = default)
    {
        var results = await context.Set<Document>()
            .Where(d => d.VestibularId == vestibularId && d.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return results;
    }

    public async Task<IReadOnlyList<DocumentChunk>> SearchSimilarChunksAsync(
        float[] queryEmbedding, int topK, Guid? vestibularId, CancellationToken cancellationToken = default)
    {
        var vector = new Vector(queryEmbedding);

        var query = context.Set<DocumentChunk>()
            .Where(c => c.DeletedAt == null);

        if (vestibularId.HasValue)
            query = query.Where(c => c.Document.VestibularId == vestibularId.Value);

        return await query
            .OrderBy(c => c.Embedding.Values.CosineDistance(vector))
            .Take(topK)
            .Include(c => c.Document)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SearchResult>> SearchAsync(
        float[] queryVector, int topK = 5, float scoreThreshold = 0.7f, CancellationToken ct = default)
    {
        var vector = new Vector(queryVector);

        var rows = await context.Set<DocumentChunk>()
            .Where(c => c.DeletedAt == null)
            .Select(c => new
            {
                Chunk = c,
                Distance = c.Embedding.Values.CosineDistance(vector)
            })
            .OrderBy(x => x.Distance)
            .Take(topK)
            .AsNoTracking()
            .ToListAsync(ct);

        return rows
            .Select(x => new SearchResult(x.Chunk, 1f - (float)x.Distance))
            .Where(r => r.Score >= scoreThreshold)
            .ToList();
    }

    public async Task UpsertAsync(Document document, CancellationToken ct = default)
    {
        var existing = await context.Set<Document>()
            .FirstOrDefaultAsync(d => d.Id == document.Id, ct);

        if (existing is null)
            await context.Set<Document>().AddAsync(document, ct);
        else
            context.Set<Document>().Update(document);
    }
}
