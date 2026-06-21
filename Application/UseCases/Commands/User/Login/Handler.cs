using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.UseCases.Commands.User.Login;

internal class Handler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    ITokenService tokenService,
    IDbCommit dbCommit,
    IMapper mapper)
    : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var user = mapper.Map<Domain.Entities.Core.User>(request);
        var userFromDb = await userRepository.Authenticate(user, cancellationToken);

        if (userFromDb is null)
            return new Response(404, Message: "Usuário não encontrado ou não ativado");

        if (!userFromDb.Password.VerifyPassword(user.Password.Content, userFromDb.Password.Salt))
            return new Response(403, Message: "Senha inválida");

        await refreshTokenRepository.DeleteAllUserTokensAsync(userFromDb.Id, cancellationToken);

        var (accessToken, _, accessExpiresAt) = tokenService.GenerateAccessToken(userFromDb);
        var (refreshToken, refreshJwtId, refreshExpiresAt) = tokenService.GenerateRefreshToken(userFromDb);

        await tokenService.CreateRefreshTokenEntityAsync(
            userFromDb,
            refreshToken,
            refreshJwtId,
            refreshExpiresAt,
            request.DeviceInfo,
            request.IpAddress,
            cancellationToken);

        await dbCommit.Commit(cancellationToken);

        userFromDb.AssignToken(accessToken);

        return new Response(
            StatusCode: 200,
            Message: "Login realizado com sucesso",
            Notifications: null,
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            AccessTokenExpiresAt: accessExpiresAt,
            RefreshTokenExpiresAt: refreshExpiresAt,
            User: mapper.Map<ResponseUser>(userFromDb));
    }
}