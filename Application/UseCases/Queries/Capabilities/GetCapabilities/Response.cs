using Application.Dtos.Capabilities;
using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Queries.Capabilities.GetCapabilities;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    CapabilitiesResponse? Capabilities = null) : BaseResponse(StatusCode, Message, Notifications);
