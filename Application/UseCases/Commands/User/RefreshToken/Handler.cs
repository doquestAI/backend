using Domain.Interfaces.Services;
using MediatR;

namespace Application.UseCases.Commands.User.RefreshToken;

internal class Handler(ITokenService tokenService) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var result = await tokenService.RefreshTokensAsync(
            request.RefreshToken,
            request.DeviceInfo,
            request.IpAddress,
            cancellationToken);

        if (result == null)
            return new Response(401, "Invalid or expired refresh token");

        var (accessToken, refreshToken, accessExpiresAt, refreshExpiresAt) = result.Value;

        return new Response(
            200,
            "Token refreshed successfully",
            null,
            accessToken,
            refreshToken,
            accessExpiresAt,
            refreshExpiresAt);
    }
}