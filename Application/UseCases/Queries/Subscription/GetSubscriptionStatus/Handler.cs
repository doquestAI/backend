using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Queries.Subscription.GetSubscriptionStatus;

internal class Handler(
    IAccountSubscriptionRepository subscriptionRepository) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var entraId = new EntraUserId(request.EntraUserId);
        if (!entraId.IsValid)
            return new Response(400, "Invalid request", entraId.Notifications.ToList());

        var subscription = await subscriptionRepository.GetByEntraUserIdAsync(entraId, cancellationToken);
        if (subscription is null)
            return new Response(404, "Subscription not found");

        return new Response(
            StatusCode: 200,
            EntraUserId: subscription.EntraUserId.Value,
            StripeCustomerId: subscription.StripeCustomerId.Value,
            Status: subscription.Status.Value,
            PlanId: subscription.PlanId);
    }
}