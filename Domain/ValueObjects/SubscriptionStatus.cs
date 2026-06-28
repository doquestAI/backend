using Flunt.Br;

namespace Domain.ValueObjects;

internal sealed class SubscriptionStatus : BaseValueObject
{
    internal static readonly string Active = "active";
    internal static readonly string Cancelled = "cancelled";
    internal static readonly string Paused = "paused";
    internal static readonly string PastDue = "past_due";

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