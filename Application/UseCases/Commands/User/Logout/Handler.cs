using Domain.Common.Responses;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.UseCases.Commands.User.Logout;

internal class Handler(
    ITokenService tokenService,
    IRefreshTokenRepository refreshTokenRepository,
    IDbCommit dbCommit) : IRequestHandler<Request, BaseResponse>
{
    public async Task<BaseResponse> Handle(Request request, CancellationToken cancellationToken)
    {
        var userId = tokenService.GetUserIdFromToken(request.AccessToken);

        if (userId == null)
            return new BaseResponse(400, "Invalid token");

        var principal = tokenService.ValidateToken(request.AccessToken);
        if (principal == null)
            return new BaseResponse(400, "Invalid or expired token");

        if (!string.IsNullOrEmpty(request.RefreshToken))
        {
            var refreshJwtId = tokenService.GetJwtIdFromToken(request.RefreshToken);
            if (refreshJwtId != null)
            {
                await refreshTokenRepository.RevokeTokenAsync(refreshJwtId, cancellationToken);
            }
        }
        else
        {
            await refreshTokenRepository.RevokeAllUserTokensAsync(userId.Value, cancellationToken);
        }

        await dbCommit.Commit(cancellationToken);

        return new BaseResponse(200, "Logout successful");
    }
}