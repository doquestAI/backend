namespace Infrastructure.Options;

internal sealed class StripeOptions
{
    public const string SectionName = "Stripe";
    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public string FreePriceId { get; set; } = string.Empty;
    public string ProPriceId { get; set; } = string.Empty;
    public string MaxPriceId { get; set; } = string.Empty;
}
