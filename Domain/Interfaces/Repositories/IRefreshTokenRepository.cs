using Domain.Entities.Core;

namespace Domain.Interfaces.Repositories;

internal interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    Task<RefreshToken?> GetByJwtIdAsync(string jwtId, CancellationToken cancellationToken);
    Task<RefreshToken?> GetValidTokenAsync(Guid userId, string jwtId, CancellationToken cancellationToken);
    Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken);
    Task RevokeTokenAsync(string jwtId, CancellationToken cancellationToken);
    Task DeleteExpiredTokensAsync(CancellationToken cancellationToken);
    Task DeleteAllUserTokensAsync(Guid userId, CancellationToken cancellationToken);
}