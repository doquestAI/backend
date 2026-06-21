using Domain.Entities.Abstracts;
using Domain.ValueObjects;

namespace Domain.Entities.Core.Subscriptions;

internal class AccountSubscription : Entity
{
    public EntraUserId EntraUserId { get; private set; } = null!;
    public StripeCustomerId StripeCustomerId { get; private set; } = null!;
    public SubscriptionStatus Status { get; private set; } = null!;
    public string PlanId { get; private set; } = string.Empty;

    public AccountSubscription() { }

    public AccountSubscription(string entraUserId, string stripeCustomerId, string planId)
    {
        var entraId = new EntraUserId(entraUserId);
        var customerId = new StripeCustomerId(stripeCustomerId);
        var status = new SubscriptionStatus(SubscriptionStatus.Active);

        AddNotificationsFromValueObjects(entraId, customerId, status);

        if (!IsValid)
            return;

        EntraUserId = entraId;
        StripeCustomerId = customerId;
        Status = status;
        PlanId = planId;
    }

    public void Activate()
    {
        var status = new SubscriptionStatus(SubscriptionStatus.Active);
        AddNotificationsFromValueObjects(status);
        if (!IsValid) return;

        Status = status;
    }

    public void Deactivate()
    {
        var status = new SubscriptionStatus(SubscriptionStatus.Cancelled);
        AddNotificationsFromValueObjects(status);
        if (!IsValid) return;

        Status = status;
    }

    public void Pause()
    {
        var status = new SubscriptionStatus(SubscriptionStatus.Paused);
        AddNotificationsFromValueObjects(status);
        if (!IsValid) return;

        Status = status;
    }
}
