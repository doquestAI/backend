using Domain.Entities.Core;

namespace Domain.Interfaces.Repositories;

internal interface IDocumentRepository : IBaseRepository<Document>
{
    Task<Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Document>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<Document>> GetPendingUploadsAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Document>> GetDocumentsNeedingSignedUrlAsync(int limit, CancellationToken cancellationToken);
    Task<IEnumerable<Document>> GetDocumentsWithExpiredSignedUrlAsync(int limit, CancellationToken cancellationToken);
}
