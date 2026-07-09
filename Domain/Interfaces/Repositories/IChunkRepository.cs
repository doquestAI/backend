using Domain.Entities.Core.Documents;

namespace Domain.Interfaces.Repositories;

internal interface IChunkRepository : IBaseRepository<Chunk>
{
    Task<List<Chunk>> GetByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken);
    Task<List<(Chunk chunk, float score)>> SearchSimilarAsync(
        ReadOnlyMemory<float> embedding,
        int topK = 10,
        float minScore = 0.0f,
        CancellationToken cancellationToken = default);
    Task CreateBatchAsync(List<Chunk> chunks, CancellationToken cancellationToken);
}
