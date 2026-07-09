using Domain.Entities.Core.Documents;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Flunt.Notifications;
using Flunt.Validations;
using MediatR;

namespace Application.UseCases.Commands.Chunk.CreateBatch;

internal class Handler(
    IChunkRepository chunkRepository,
    IEmbeddingService embeddingService,
    IDbCommit dbCommit) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var contract = new Contract<Notifiable<Notification>>()
            .Requires()
            .IsNotEmpty(request.DocumentId, "DocumentId", "DocumentId is required")
            .IsNotNull(request.Chunks, "Chunks", "Chunks is required")
            .IsGreaterThan(request.Chunks?.Count ?? 0, 0, "Chunks", "At least one chunk is required");

        if (!contract.IsValid)
            return new Response(
                StatusCode: 400,
                Message: "Request invalid",
                Notifications: contract.Notifications.ToList());

        var texts = request.Chunks.Select(c => c.Content).ToList();
        var embeddings = await embeddingService.GenerateBatchAsync(texts, cancellationToken);

        var chunks = request.Chunks
            .Select((chunkInput, index) =>
            {
                var chunk = new Chunk(request.DocumentId, chunkInput.PositionIndex, chunkInput.Content, chunkInput.Metadata);
                chunk.SetEmbedding(embeddings[index]);
                return chunk;
            })
            .ToList();

        await chunkRepository.CreateBatchAsync(chunks, cancellationToken);
        await dbCommit.Commit(cancellationToken);

        return new Response(
            StatusCode: 201,
            Message: $"Created {chunks.Count} chunks successfully",
            CreatedCount: chunks.Count,
            ChunkIds: chunks.Select(c => c.Id).ToList());
    }
}
