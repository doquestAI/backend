using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Commands.Document.Delete;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    bool Success = false) : BaseResponse(StatusCode, Message, Notifications);