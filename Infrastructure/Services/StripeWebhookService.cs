using Domain.Configurations;
using Domain.Interfaces.Services;
using Microsoft.Extensions.Options;
using Stripe;

namespace Infrastructure.Services;

internal class StripeWebhookService(IOptions<StripeSettings> stripeSettings) : IStripeWebhookService
{
    public StripeEventDto? ParseAndValidate(string rawPayload, string signatureHeader)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                rawPayload,
                signatureHeader,
                stripeSettings.Value.WebhookSecret);

            if (stripeEvent.Data?.Object is not Subscription subscription)
                return null;

            return new StripeEventDto(
                EventType: stripeEvent.Type,
                CustomerId: subscription.CustomerId,
                SubscriptionStatus: subscription.Status);
        }
        catch (StripeException)
        {
            return null;
        }
    }
}