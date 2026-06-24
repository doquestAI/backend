using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Commands.Subscription.ActivateUser;

internal class Handler(
    IAccountSubscriptionRepository subscriptionRepository,
    IIdentityProviderService identityProvider,
    IDbCommit dbCommit) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var entraId = new EntraUserId(request.EntraUserId);
        if (!entraId.IsValid)
            return new Response(400, "Invalid request", entraId.Notifications.ToList());

        var subscription = await subscriptionRepository.GetByEntraUserIdAsync(entraId, cancellationToken);
        if (subscription is null)
            return new Response(404, "Subscription not found");

        subscription.Activate();
        if (!subscription.IsValid)
            return new Response(400, "Cannot activate subscription", subscription.Notifications.ToList());

        await identityProvider.UnblockUserAsync(subscription.EntraUserId, cancellationToken);
        subscriptionRepository.Update(subscription);
        await dbCommit.Commit(cancellationToken);

        return new Response(200, "User activated successfully");
    }
}