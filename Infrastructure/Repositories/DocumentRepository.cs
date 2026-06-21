using Domain.Entities.Core;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal class DocumentRepository(CoreDbContext context)
    : BaseRepository<Document>(context), IDocumentRepository
{
    public async Task<Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Set<Document>()
            .FirstOrDefaultAsync(d => d.Id == id && d.DeletedDate == null, cancellationToken);
    }

    public async Task<IEnumerable<Document>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await context.Set<Document>()
            .AsNoTracking()
            .Where(d => d.UploadedByUserId == userId && d.DeletedDate == null)
            .OrderByDescending(d => d.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Document>> GetPendingUploadsAsync(CancellationToken cancellationToken)
    {
        return await context.Set<Document>()
            .Where(d => d.Status == DocumentStatus.Pending && d.DeletedDate == null)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Document>> GetDocumentsNeedingSignedUrlAsync(int limit, CancellationToken cancellationToken)
    {
        return await context.Set<Document>()
            .Where(d => d.DeletedDate == null &&
                        d.CloudStorageKey != null &&
                        d.SignedUrl == null &&
                        d.Status != DocumentStatus.DeletionPending &&
                        d.Status != DocumentStatus.DeletionFailed)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Document>> GetDocumentsWithExpiredSignedUrlAsync(int limit, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        return await context.Set<Document>()
            .Where(d => d.DeletedDate == null &&
                        d.CloudStorageKey != null &&
                        d.SignedUrlExpiration != null &&
                        d.SignedUrlExpiration <= now &&
                        d.Status != DocumentStatus.DeletionPending &&
                        d.Status != DocumentStatus.DeletionFailed)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}
