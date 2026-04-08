using Domain.Interfaces.Services;
using Domain.Interfaces.Services.Payment;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Infrastructure.Services.Payments;

internal sealed class StripeService : IGatewayPaymentService
{
    private readonly StripeOptions _options;

    public StripeService(IOptions<StripeOptions> options)
    {
        _options = options.Value;
        StripeConfiguration.ApiKey = _options.SecretKey;
    }

    public async Task<string> CreateCheckoutSessionAsync(
        string userEmail,
        string stripePriceId,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken = default)
    {
        var sessionOptions = new SessionCreateOptions
        {
            CustomerEmail = userEmail,
            PaymentMethodTypes = ["card"],
            LineItems =
            [
                new SessionLineItemOptions
                {
                    Price = stripePriceId,
                    Quantity = 1
                }
            ],
            Mode = "subscription",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl
        };

        var service = new SessionService();
        var session = await service.CreateAsync(sessionOptions, cancellationToken: cancellationToken);
        return session.Url;
    }

    public Task<string?> GetProductIdFromEventAsync(
        string webhookPayload,
        string stripeSignature,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                webhookPayload,
                stripeSignature,
                _options.WebhookSecret);

            if (stripeEvent.Data.Object is Subscription subscription)
            {
                var productId = subscription.Items.Data.FirstOrDefault()?.Price.ProductId;
                return Task.FromResult(productId);
            }

            return Task.FromResult<string?>(null);
        }
        catch (StripeException)
        {
            return Task.FromResult<string?>(null);
        }
    }
}