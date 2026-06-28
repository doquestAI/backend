using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Commands.Capabilities.ConnectMcp;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    string? McpName = null) : BaseResponse(StatusCode, Message, Notifications);
