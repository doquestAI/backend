using Domain.Entities.Core;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal class RefreshTokenRepository(CoreDbContext context)
    : BaseRepository<RefreshToken>(context), IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByJwtIdAsync(string jwtId, CancellationToken cancellationToken)
    {
        return await context.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.JwtId == jwtId, cancellationToken);
    }

    public async Task<RefreshToken?> GetValidTokenAsync(Guid userId, string jwtId, CancellationToken cancellationToken)
    {
        return await context.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt =>
                rt.UserId == userId &&
                rt.JwtId == jwtId &&
                !rt.IsRevoked &&
                rt.ExpiresAt > DateTime.UtcNow,
                cancellationToken);
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        var tokens = await context.Set<RefreshToken>()
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.Revoke();
        }
    }

    public async Task RevokeTokenAsync(string jwtId, CancellationToken cancellationToken)
    {
        var token = await context.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.JwtId == jwtId, cancellationToken);

        token?.Revoke();
    }

    public async Task DeleteExpiredTokensAsync(CancellationToken cancellationToken)
    {
        var expiredTokens = await context.Set<RefreshToken>()
            .Where(rt => rt.ExpiresAt < DateTime.UtcNow || rt.IsRevoked)
            .ToListAsync(cancellationToken);

        context.Set<RefreshToken>().RemoveRange(expiredTokens);
    }

    public async Task DeleteAllUserTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        var tokens = await context.Set<RefreshToken>()
            .Where(rt => rt.UserId == userId)
            .ToListAsync(cancellationToken);

        context.Set<RefreshToken>().RemoveRange(tokens);
    }
}