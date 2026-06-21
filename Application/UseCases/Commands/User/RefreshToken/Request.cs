using Domain.Common.Responses;
using Flunt.Notifications;
using MediatR;

namespace Application.UseCases.Commands.User.RefreshToken;

internal record Request(
    string RefreshToken,
    string? DeviceInfo = null,
    string? IpAddress = null) : IRequest<Response>;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    string? AccessToken = null,
    string? RefreshToken = null,
    DateTime? AccessTokenExpiresAt = null,
    DateTime? RefreshTokenExpiresAt = null)
    : BaseResponse(StatusCode, Message, Notifications);