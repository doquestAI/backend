using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal class DocumentRepository(CoreDbContext context)
    : BaseRepository<Document>(context), IDocumentRepository
{
    public async Task<Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Set<Document>()
            .FirstOrDefaultAsync(d => d.Id == id && d.DeletedAt == null, cancellationToken);
    }
    public Task<IReadOnlyList<Document>> GetByVestibularIdAsync(Guid vestibularId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<DocumentChunk>> SearchSimilarChunksAsync(float[] queryEmbedding, int topK, Guid? vestibularId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
