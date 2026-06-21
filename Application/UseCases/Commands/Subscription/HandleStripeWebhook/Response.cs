using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Commands.Subscription.HandleStripeWebhook;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null) : BaseResponse(StatusCode, Message, Notifications);
