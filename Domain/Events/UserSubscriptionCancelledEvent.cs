namespace Domain.Events;

internal record UserSubscriptionCancelledEvent(string EntraUserId, DateTime OccurredAt);