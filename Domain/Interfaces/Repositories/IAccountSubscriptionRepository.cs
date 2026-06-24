using Domain.Entities.Core.Subscriptions;
using Domain.ValueObjects;

namespace Domain.Interfaces.Repositories;

internal interface IAccountSubscriptionRepository : IBaseRepository<AccountSubscription>
{
    Task<AccountSubscription?> GetByEntraUserIdAsync(EntraUserId entraUserId, CancellationToken cancellationToken = default);
    Task<AccountSubscription?> GetByStripeCustomerIdAsync(StripeCustomerId stripeCustomerId, CancellationToken cancellationToken = default);
}