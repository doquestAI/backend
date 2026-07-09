using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Queries.Chunk.Search;

internal record ChunkResult(Guid ChunkId, Guid DocumentId, int PositionIndex, string Content, float Score);

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    List<ChunkResult>? Results = null) : BaseResponse(StatusCode, Message, Notifications);
