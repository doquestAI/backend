using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Commands.Subscription.HandleStripeWebhook;

internal class Handler(
    IStripeWebhookService stripeWebhook,
    IAccountSubscriptionRepository subscriptionRepository,
    IIdentityProviderService identityProvider,
    IDbCommit dbCommit) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var stripeEvent = stripeWebhook.ParseAndValidate(request.RawPayload, request.StripeSignatureHeader);
        if (stripeEvent is null)
            return new Response(400, "Invalid Stripe webhook signature");

        var customerId = new StripeCustomerId(stripeEvent.CustomerId);
        if (!customerId.IsValid)
            return new Response(400, "Invalid customer ID in Stripe event");

        var subscription = await subscriptionRepository.GetByStripeCustomerIdAsync(customerId, cancellationToken);
        if (subscription is null)
            return new Response(404, "No subscription found for this Stripe customer");

        switch (stripeEvent.SubscriptionStatus)
        {
            case "active":
                subscription.Activate();
                if (!subscription.IsValid)
                    return new Response(400, "Cannot activate", subscription.Notifications.ToList());
                await identityProvider.UnblockUserAsync(subscription.EntraUserId, cancellationToken);
                break;

            case "canceled":
            case "unpaid":
                subscription.Deactivate();
                if (!subscription.IsValid)
                    return new Response(400, "Cannot deactivate", subscription.Notifications.ToList());
                await identityProvider.BlockUserAsync(subscription.EntraUserId, cancellationToken);
                break;

            case "paused":
                subscription.Pause();
                if (!subscription.IsValid)
                    return new Response(400, "Cannot pause", subscription.Notifications.ToList());
                await identityProvider.BlockUserAsync(subscription.EntraUserId, cancellationToken);
                break;
        }

        subscriptionRepository.Update(subscription);
        await dbCommit.Commit(cancellationToken);

        return new Response(200, "Webhook processed");
    }
}