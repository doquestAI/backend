using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Queries.Subscription.GetSubscriptionStatus;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    string? EntraUserId = null,
    string? StripeCustomerId = null,
    string? Status = null,
    string? PlanId = null) : BaseResponse(StatusCode, Message, Notifications);
