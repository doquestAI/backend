using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Commands.Enem.AskHelper;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    string? Answer = null) : BaseResponse(StatusCode, Message, Notifications);
