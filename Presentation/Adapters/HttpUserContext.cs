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
            var userIdClaim = User.FindFirst("user_id")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return userIdClaim != null
                ? Guid.Parse(userIdClaim)
                : throw new UnauthorizedAccessException("Usuário não autenticado");
        }
    }

    public string Role
    {
        get
        {
            return User.FindFirst("role")?.Value
                ?? User.FindFirst(ClaimTypes.Role)?.Value
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
