namespace Domain.Interfaces.Services;

internal interface IStripeWebhookService
{
    StripeEventDto? ParseAndValidate(string rawPayload, string signatureHeader);
}

internal record StripeEventDto(string EventType, string CustomerId, string SubscriptionStatus);