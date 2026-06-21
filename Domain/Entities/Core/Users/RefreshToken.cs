using Domain.Entities.Abstracts;

namespace Domain.Entities.Core;

internal class RefreshToken : Entity
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public string JwtId { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public string? DeviceInfo { get; private set; }
    public string? IpAddress { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public User? User { get; private set; }

    private RefreshToken() { }

    public RefreshToken(
        Guid userId,
        string tokenHash,
        string jwtId,
        DateTime expiresAt,
        string? deviceInfo = null,
        string? ipAddress = null)
    {
        UserId = userId;
        TokenHash = tokenHash;
        JwtId = jwtId;
        ExpiresAt = expiresAt;
        DeviceInfo = deviceInfo;
        IpAddress = ipAddress;
        IsRevoked = false;
    }

    public void Revoke()
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }

    public new bool IsValid()
        => !IsRevoked && ExpiresAt > DateTime.UtcNow;
}