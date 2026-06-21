using Flunt.Notifications;
using Flunt.Validations;

namespace Domain.ValueObjects;

internal class SubscriptionPeriod : Notifiable<Notification>
{
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    public SubscriptionPeriod(DateTime startDate, DateTime? endDate = null)
    {
        StartDate = startDate;
        EndDate = endDate;

        AddNotifications(new Contract<Notification>()
            .Requires()
            .IsLowerOrEqualsThan(startDate, endDate ?? DateTime.MaxValue,
                "SubscriptionPeriod.EndDate", "Data final deve ser posterior à inicial")
        );
    }
}