using Domain.ValueObjects;

namespace Domain.Interfaces.Services;

internal interface IIdentityProviderService
{
    Task BlockUserAsync(EntraUserId userId, CancellationToken cancellationToken = default);
    Task UnblockUserAsync(EntraUserId userId, CancellationToken cancellationToken = default);
}
