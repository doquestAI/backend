using Domain.Interfaces.Context;
using System.Security.Claims;

namespace Presentation.Adapters;

public class HttpUserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException("Usuário não autenticado");

    public Guid UserId
    {
        get
        {
            // Entra External ID uses the "oid" claim (object ID) mapped to NameIdentifier
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("oid")?.Value;

            return userIdClaim != null && Guid.TryParse(userIdClaim, out var guid)
                ? guid
                : throw new UnauthorizedAccessException("Usuário não autenticado");
        }
    }

    public string Role
    {
        get
        {
            return User.FindFirst(ClaimTypes.Role)?.Value
                ?? User.FindFirst("roles")?.Value
                ?? "User";
        }
    }

    public string Email
    {
        get
        {
            return User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst("email")?.Value
                ?? throw new UnauthorizedAccessException("Email não encontrado");
        }
    }
}
