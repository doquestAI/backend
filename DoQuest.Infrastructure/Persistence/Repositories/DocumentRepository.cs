using System.Globalization;
using DoQuest.Domain.Entities;
using DoQuest.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DoQuest.Infrastructure.Persistence.Repositories;

public sealed class DocumentRepository(DoQuestDbContext context)
    : Repository<Document>(context), IDocumentRepository
{
    public async Task<IReadOnlyList<Document>> GetByVestibularIdAsync(
        Guid vestibularId,
        CancellationToken cancellationToken = default) =>
        await Context.Documents
            .Include(d => d.Chunks)
            .Where(d => d.VestibularId == vestibularId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<DocumentChunk>> SearchSimilarChunksAsync(
        float[] queryEmbedding,
        int topK,
        Guid? vestibularId,
        CancellationToken cancellationToken = default)
    {
        // Build vector literal for pgvector — safe because values are float (no SQL injection risk)
        var vectorLiteral = string.Join(",",
            queryEmbedding.Select(f => f.ToString("G9", CultureInfo.InvariantCulture)));

        var vestibularFilter = vestibularId.HasValue
            ? $"AND d.id = dc.document_id AND d.vestibular_id = '{vestibularId.Value}'"
            : string.Empty;

        var joinClause = vestibularId.HasValue
            ? "JOIN documents d ON d.id = dc.document_id"
            : string.Empty;

        var sql = $"""
            SELECT dc.*
            FROM document_chunks dc
            {joinClause}
            WHERE 1=1 {vestibularFilter}
            ORDER BY dc.embedding <-> '[{vectorLiteral}]'::vector
            LIMIT {topK}
            """;

        var chunks = await Context.DocumentChunks
            .FromSqlRaw(sql)
            .Include(c => c.Document)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return chunks.AsReadOnly();
    }
}
