using Domain.Entities.Core.Documents;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Flunt.Notifications;
using Flunt.Validations;
using MediatR;

namespace Application.UseCases.Commands.Chunk.Create;

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
            .IsNotNull(request.Content, "Content", "Content is required")
            .IsGreaterThan(request.Content?.Length ?? 0, 0, "Content", "Content cannot be empty");

        if (!contract.IsValid)
            return new Response(
                StatusCode: 400,
                Message: "Request invalid",
                Notifications: contract.Notifications.ToList());

        var chunk = new Chunk(request.DocumentId, request.PositionIndex, request.Content, request.Metadata);

        var embedding = await embeddingService.GenerateAsync(request.Content, cancellationToken);
        chunk.SetEmbedding(embedding);

        await chunkRepository.CreateAsync(chunk, cancellationToken);
        await dbCommit.Commit(cancellationToken);

        return new Response(
            StatusCode: 201,
            Message: "Chunk created successfully",
            ChunkId: chunk.Id);
    }
}
