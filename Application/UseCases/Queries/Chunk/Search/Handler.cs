using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Flunt.Notifications;
using Flunt.Validations;
using MediatR;

namespace Application.UseCases.Queries.Chunk.Search;

internal class Handler(
    IChunkRepository chunkRepository,
    IEmbeddingService embeddingService) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var contract = new Contract<Notifiable<Notification>>()
            .Requires()
            .IsNotNull(request.Query, "Query", "Query is required")
            .IsGreaterThan(request.Query?.Length ?? 0, 0, "Query", "Query cannot be empty");

        if (!contract.IsValid)
            return new Response(
                StatusCode: 400,
                Message: "Request invalid",
                Notifications: contract.Notifications.ToList());

        var queryEmbedding = await embeddingService.GenerateAsync(request.Query, cancellationToken);

        var results = await chunkRepository.SearchSimilarAsync(
            queryEmbedding,
            request.TopK,
            request.MinScore,
            cancellationToken);

        var chunkResults = results
            .Select(r => new ChunkResult(
                ChunkId: r.chunk.Id,
                DocumentId: r.chunk.DocumentId,
                PositionIndex: r.chunk.PositionIndex,
                Content: r.chunk.Content,
                Score: r.score))
            .ToList();

        return new Response(
            StatusCode: 200,
            Results: chunkResults);
    }
}
