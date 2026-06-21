using Domain.Entities.Core;
using System.Security.Claims;

namespace Domain.Interfaces.Services;

internal interface ITokenService
{
    (string AccessToken, string JwtId, DateTime ExpiresAt) GenerateAccessToken(User user);

    (string RefreshToken, string JwtId, DateTime ExpiresAt) GenerateRefreshToken(User user);

    ClaimsPrincipal? ValidateToken(string token);

    string? GetJwtIdFromToken(string token);

    Guid? GetUserIdFromToken(string token);

    string? GetTokenTypeFromToken(string token);

    DateTime? GetTokenIssuedAtFromToken(string token);

    DateTime? GetTokenExpirationFromToken(string token);

    string ComputeTokenHash(string token);

    Task<(string AccessToken, string RefreshToken, DateTime AccessExpiresAt, DateTime RefreshExpiresAt)?> RefreshTokensAsync(
        string refreshToken,
        string? deviceInfo,
        string? ipAddress,
        CancellationToken cancellationToken);

    Task<RefreshToken> CreateRefreshTokenEntityAsync(
        User user,
        string refreshTokenJwt,
        string jwtId,
        DateTime expiresAt,
        string? deviceInfo,
        string? ipAddress,
        CancellationToken cancellationToken);
}