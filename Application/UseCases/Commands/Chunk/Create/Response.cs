using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Commands.Chunk.Create;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    Guid ChunkId = default) : BaseResponse(StatusCode, Message, Notifications);
