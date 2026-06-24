using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Commands.Enem.GradeAnswer;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    bool? IsCorrect = null,
    string? Explanation = null,
    float? Score = null) : BaseResponse(StatusCode, Message, Notifications);
