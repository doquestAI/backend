using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Commands.User.Register;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    string? Token = null) : BaseResponse(StatusCode, Message, Notifications);