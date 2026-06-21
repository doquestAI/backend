using Domain.Common.Responses;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Commands.User.Delete;

internal class Handler(
    IUserRepository userRepository,
    IDbCommit dbCommit
)
    : IRequestHandler<DeleteUserRequest, BaseResponse>
{
    private readonly IDbCommit _dbCommit = dbCommit;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<BaseResponse> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        var userExists = await _userRepository.GetWithParametersAsyncWithTracking(
            u => u.Id.Equals(request.Id),
            cancellationToken
        );

        if (userExists is null)
            return new BaseResponse(404, "Usuário não encontrado!");

        _userRepository.Delete(userExists);
        await _dbCommit.Commit(cancellationToken);
        return new BaseResponse(200, "Usuário deletado com sucesso!");
    }
}