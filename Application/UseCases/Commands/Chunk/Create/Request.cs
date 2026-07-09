using MediatR;

namespace Application.UseCases.Commands.Chunk.Create;

internal record Request(
    Guid DocumentId,
    int PositionIndex,
    string Content,
    string? Metadata = null) : IRequest<Response>;
