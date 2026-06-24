namespace Domain.Events;

internal record UserSubscriptionActivatedEvent(string EntraUserId, DateTime OccurredAt);