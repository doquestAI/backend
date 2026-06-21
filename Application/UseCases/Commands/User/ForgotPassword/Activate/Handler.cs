using Domain.Common.Responses;
using Domain.Configurations;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.UseCases.Commands.User.ForgotPassword.Activate;

internal class Handler(
    IUserRepository userRepository,
    IDbCommit dbCommit,
    IRefreshTokenRepository refreshTokenRepository,
    IOptions<AppSettings> appSettings) : IRequestHandler<Request, BaseResponse>
{
    public async Task<BaseResponse> Handle(Request request, CancellationToken cancellationToken)
    {
        var userFromDb = await userRepository.GetByEmail(request.email, cancellationToken);

        if (userFromDb is null || userFromDb.TokenActivate is null)
            return new BaseResponse(statusCode: 400, message: "Invalid or expired token");

        var plainToken = request.token?.ToString();
        if (string.IsNullOrWhiteSpace(plainToken) || !userFromDb.TokenActivate.Validate(plainToken, appSettings.Value.EncryptionKey))
            return new BaseResponse(statusCode: 400, message: "Invalid or expired token");

        userFromDb.UpdatePassword(new Password(request.newPassword));
        if (!userFromDb.IsValid)
            return new BaseResponse(statusCode: 400, message: "Invalid password", notifications: userFromDb.Notifications.ToList());

        userFromDb.ClearToken();
        userRepository.Update(userFromDb);

        await refreshTokenRepository.RevokeAllUserTokensAsync(userFromDb.Id, cancellationToken);
        await dbCommit.Commit(cancellationToken);

        return new BaseResponse(statusCode: 200, message: "Password reset successful");
    }
}