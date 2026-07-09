using MediatR;

namespace Application.UseCases.Commands.Chunk.CreateBatch;

internal record ChunkInput(int PositionIndex, string Content, string? Metadata = null);

internal record Request(Guid DocumentId, List<ChunkInput> Chunks) : IRequest<Response>;
