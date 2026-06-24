using Domain.Interfaces.Services;
using Domain.ValueObjects;
using Microsoft.Graph;

namespace Infrastructure.Services;

internal class EntraIdentityProviderService(GraphServiceClient graphClient) : IIdentityProviderService
{
    public async Task BlockUserAsync(EntraUserId userId, CancellationToken cancellationToken = default)
        => await graphClient.Users[userId.Value]
            .Request()
            .UpdateAsync(new User { AccountEnabled = false }, cancellationToken);

    public async Task UnblockUserAsync(EntraUserId userId, CancellationToken cancellationToken = default)
        => await graphClient.Users[userId.Value]
            .Request()
            .UpdateAsync(new User { AccountEnabled = true }, cancellationToken);
}