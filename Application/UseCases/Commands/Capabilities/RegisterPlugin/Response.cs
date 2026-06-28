using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Commands.Capabilities.RegisterPlugin;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    string? PluginName = null) : BaseResponse(StatusCode, Message, Notifications);
