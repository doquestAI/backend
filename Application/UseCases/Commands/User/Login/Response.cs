using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Commands.User.Login;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    string? AccessToken = null,
    string? RefreshToken = null,
    DateTime? AccessTokenExpiresAt = null,
    DateTime? RefreshTokenExpiresAt = null,
    ResponseUser? User = null) : BaseResponse(StatusCode, Message, Notifications);