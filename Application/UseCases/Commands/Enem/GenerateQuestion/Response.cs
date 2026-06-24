using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Commands.Enem.GenerateQuestion;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    string? Statement = null,
    IReadOnlyList<string>? Options = null,
    string? CorrectKey = null,
    string? Explanation = null,
    string? Area = null) : BaseResponse(StatusCode, Message, Notifications);
