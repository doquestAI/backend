using Domain.Entities.Abstracts;
using Domain.ValueObjects;

namespace Domain.Entities.Payments;

internal class FreeSubscription : Subscription
{
    public FreeSubscription(Guid studentId, SubscriptionPeriod period)
        : base(studentId, period)
    {
    }
    protected FreeSubscription() : base(Guid.Empty, new SubscriptionPeriod(DateTime.MinValue, DateTime.MinValue))
    {
    }
}