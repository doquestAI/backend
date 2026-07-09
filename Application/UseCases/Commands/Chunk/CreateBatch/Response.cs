using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Commands.Chunk.CreateBatch;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    int CreatedCount = 0,
    List<Guid>? ChunkIds = null) : BaseResponse(StatusCode, Message, Notifications);
