using Flunt.Br;

namespace Domain.ValueObjects;

internal sealed class SubscriptionStatus : BaseValueObject
{
    public static readonly string Active = "active";
    public static readonly string Cancelled = "cancelled";
    public static readonly string Paused = "paused";
    public static readonly string PastDue = "past_due";

    private static readonly string[] AllowedValues = ["active", "cancelled", "paused", "past_due"];

    public string Value { get; private set; } = string.Empty;

    public SubscriptionStatus() { }

    public SubscriptionStatus(string? value)
    {
        AddNotifications(
            new Contract()
                .Requires()
                .IsNotNullOrEmpty(value, nameof(SubscriptionStatus), "SubscriptionStatus cannot be null or empty")
                .IsTrue(value != null && AllowedValues.Contains(value), nameof(SubscriptionStatus),
                    $"SubscriptionStatus must be one of: {string.Join(", ", AllowedValues)}")
        );

        if (!IsValid)
            return;

        Value = value!;
    }

    public bool IsActive() => Value == Active;
    public bool IsCancelled() => Value == Cancelled;
}