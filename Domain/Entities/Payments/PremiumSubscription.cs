using Domain.Entities.Abstracts;
using Domain.ValueObjects;
using Flunt.Validations;

namespace Domain.Entities.Payments;

internal class PremiumSubscription : Subscription
{
    public decimal Price { get; private set; }
    public string StripeSubscriptionId { get; private set; }
    public string StripeCustomerId { get; private set; }

    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    protected PremiumSubscription() : base(Guid.Empty, new SubscriptionPeriod(DateTime.MinValue, DateTime.MinValue))
    {
    }
    public PremiumSubscription(
        Guid studentId,
        SubscriptionPeriod period,
        decimal price,
        PaymentDetails payment,
        string stripeSubscriptionId,
        string stripeCustomerId,
        DateTime? startDate,
        DateTime? endDate
    ) : base(studentId, period, payment)
    {
        Price = price;
        StripeSubscriptionId = stripeSubscriptionId;
        StripeCustomerId = stripeCustomerId;
        StartDate = startDate;
        EndDate = endDate;

        AddNotifications(new Contract<PremiumSubscription>()
            .IsNotNullOrWhiteSpace(StripeSubscriptionId, "StripeSubscriptionId", "ID da assinatura do Stripe obrigatório")
            .IsNotNullOrWhiteSpace(StripeCustomerId, "StripeCustomerId", "ID do cliente Stripe obrigatório")
        );
    }
}