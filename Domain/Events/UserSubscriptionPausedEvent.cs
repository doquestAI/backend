namespace Domain.Events;

internal record UserSubscriptionPausedEvent(string EntraUserId, DateTime OccurredAt);