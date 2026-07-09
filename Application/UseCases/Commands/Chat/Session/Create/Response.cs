using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Commands.Chat.Session.Create;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    Guid SessionId = default) : BaseResponse(StatusCode, Message, Notifications);
