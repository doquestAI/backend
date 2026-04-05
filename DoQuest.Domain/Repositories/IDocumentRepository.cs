using DoQuest.Domain.Entities;

namespace DoQuest.Domain.Repositories;

public interface IDocumentRepository : IRepository<Document>
{
    Task<IReadOnlyList<Document>> GetByVestibularIdAsync(Guid vestibularId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a semantic similarity search using pgvector (cosine distance).
    /// Returns the top-K most relevant chunks.
    /// </summary>
    Task<IReadOnlyList<DocumentChunk>> SearchSimilarChunksAsync(
        float[] queryEmbedding,
        int topK,
        Guid? vestibularId,
        CancellationToken cancellationToken = default);
}
