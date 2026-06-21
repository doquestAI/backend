using Domain.Configurations;
using Domain.Entities.Core;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services;

internal sealed class TokenService(
    IOptions<JwtSettings> jwtSettings,
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IDbCommit dbCommit) : ITokenService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public (string AccessToken, string JwtId, DateTime ExpiresAt) GenerateAccessToken(User user)
    {
        var jwtId = Guid.NewGuid().ToString();
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
        var token = GenerateJwtToken(user, jwtId, expiresAt, "access");
        return (token, jwtId, expiresAt);
    }

    public (string RefreshToken, string JwtId, DateTime ExpiresAt) GenerateRefreshToken(User user)
    {
        var jwtId = Guid.NewGuid().ToString();
        var expiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
        var token = GenerateJwtToken(user, jwtId, expiresAt, "refresh");
        return (token, jwtId, expiresAt);
    }

    private string GenerateJwtToken(User user, string jwtId, DateTime expiresAt, string tokenType)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email.Address!),
            new(ClaimTypes.Name, $"{user.FullName.FirstName} {user.FullName.LastName}"),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, jwtId),
            new("user_id", user.Id.ToString()),
            new("token_type", tokenType)
        };

        if (tokenType == "access")
        {
            foreach (var userRole in user.UserRoles)
            {
                var roleSlug = userRole.Role.Slug;
                claims.Add(new Claim("role", roleSlug));
            }
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }

    public string ComputeTokenHash(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }

    public string? GetJwtIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
        }
        catch
        {
            return null;
        }
    }

    public Guid? GetUserIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            return userIdClaim != null ? Guid.Parse(userIdClaim) : null;
        }
        catch
        {
            return null;
        }
    }

    public string? GetTokenTypeFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value;
        }
        catch
        {
            return null;
        }
    }

    public DateTime? GetTokenIssuedAtFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidFrom;
        }
        catch
        {
            return null;
        }
    }

    public DateTime? GetTokenExpirationFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo;
        }
        catch
        {
            return null;
        }
    }

    public async Task<(string AccessToken, string RefreshToken, DateTime AccessExpiresAt, DateTime RefreshExpiresAt)?> RefreshTokensAsync(
        string refreshToken,
        string? deviceInfo,
        string? ipAddress,
        CancellationToken cancellationToken)
    {
        var tokenType = GetTokenTypeFromToken(refreshToken);
        if (tokenType != "refresh")
            return null;

        var principal = ValidateToken(refreshToken);
        if (principal == null)
            return null;

        var jwtId = GetJwtIdFromToken(refreshToken);
        var userId = GetUserIdFromToken(refreshToken);

        if (jwtId == null || userId == null)
            return null;

        var storedToken = await refreshTokenRepository.GetValidTokenAsync(userId.Value, jwtId, cancellationToken);
        if (storedToken == null)
            return null;

        var tokenHash = ComputeTokenHash(refreshToken);
        if (storedToken.TokenHash != tokenHash)
            return null;

        var user = await userRepository.GetByIdWithRolesAsync(userId.Value, cancellationToken);

        if (user is null || !user.Active)
            return null;

        await refreshTokenRepository.DeleteAllUserTokensAsync(user.Id, cancellationToken);

        var (accessToken, _, accessExpiresAt) = GenerateAccessToken(user);
        var (newRefreshToken, newJwtId, refreshExpiresAt) = GenerateRefreshToken(user);

        await CreateRefreshTokenEntityAsync(
                user,
                newRefreshToken,
                newJwtId,
                refreshExpiresAt,
                deviceInfo ?? storedToken.DeviceInfo,
                ipAddress ?? storedToken.IpAddress,
                cancellationToken);

        await dbCommit.Commit(cancellationToken);
        return (accessToken, newRefreshToken, accessExpiresAt, refreshExpiresAt);
    }

    public async Task<RefreshToken> CreateRefreshTokenEntityAsync(
        User user,
        string refreshTokenJwt,
        string jwtId,
        DateTime expiresAt,
        string? deviceInfo,
        string? ipAddress,
        CancellationToken cancellationToken)
    {
        var tokenHash = ComputeTokenHash(refreshTokenJwt);
        var refreshTokenEntity = new RefreshToken(
            user.Id,
            tokenHash,
            jwtId,
            expiresAt,
            deviceInfo,
            ipAddress);

        await refreshTokenRepository.CreateAsync(refreshTokenEntity, cancellationToken);
        return refreshTokenEntity;
    }
}