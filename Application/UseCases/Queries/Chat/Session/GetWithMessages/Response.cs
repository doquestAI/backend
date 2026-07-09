using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Queries.Chat.Session.GetWithMessages;

internal record ChatMessageDto(Guid MessageId, string Role, string Content, DateTime CreatedAt);

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    Guid SessionId = default,
    string? Title = null,
    string? Description = null,
    DateTime? EndedAt = null,
    List<ChatMessageDto>? Messages = null) : BaseResponse(StatusCode, Message, Notifications);
