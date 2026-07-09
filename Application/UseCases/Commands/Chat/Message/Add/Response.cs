using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Commands.Chat.Message.Add;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    Guid MessageId = default) : BaseResponse(StatusCode, Message, Notifications);
