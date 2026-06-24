using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Commands.Enem.ExplainTopic;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    string? Explanation = null) : BaseResponse(StatusCode, Message, Notifications);
