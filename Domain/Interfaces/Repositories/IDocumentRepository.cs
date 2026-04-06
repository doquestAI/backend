using Domain.Entities;

namespace Domain.Interfaces.Repositories;

internal interface IDocumentRepository : IBaseRepository<Document>
{
    Task<IReadOnlyList<Document>> GetByVestibularIdAsync(Guid vestibularId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DocumentChunk>> SearchSimilarChunksAsync(
        float[] queryEmbedding,
        int topK,
        Guid? vestibularId,
        CancellationToken cancellationToken = default);
    Task<Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

}
