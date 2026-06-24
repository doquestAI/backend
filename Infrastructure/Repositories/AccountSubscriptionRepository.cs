using Domain.Entities.Core.Subscriptions;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal class AccountSubscriptionRepository(CoreDbContext context)
    : BaseRepository<AccountSubscription>(context), IAccountSubscriptionRepository
{
    public async Task<AccountSubscription?> GetByEntraUserIdAsync(
        EntraUserId entraUserId,
        CancellationToken cancellationToken = default)
        => await context.AccountSubscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.EntraUserId.Value == entraUserId.Value, cancellationToken);

    public async Task<AccountSubscription?> GetByStripeCustomerIdAsync(
        StripeCustomerId stripeCustomerId,
        CancellationToken cancellationToken = default)
        => await context.AccountSubscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StripeCustomerId.Value == stripeCustomerId.Value, cancellationToken);
}