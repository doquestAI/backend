using Domain.Common.Responses;
using Domain.Interfaces.Repositories;
using MediatR;
using System;

namespace Application.UseCases.Commands.User.Update;

internal class Handler(
    IUserRepository userRepository,
    IDbCommit dbCommit
)
    : IRequestHandler<UpdateUserRequest, BaseResponse>
{
    private readonly IDbCommit _dbCommit = dbCommit;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<BaseResponse> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetWithParametersAsyncWithTracking(
            u => u.Id.Equals(request.Id),
            cancellationToken
        );

        if (user is null)
            return new BaseResponse(404, "Usuário não encontrado!");

        user.Update(
            request.FirstName,
            request.LastName,
            request.Road,
            request.NeighBordHood,
            request.Number,
            request.CEP,
            request.Complement,
            request.Active
        );

        if (!user.IsValid)
            return new BaseResponse(400, "Dados inválidos para atualização do usuário", user.Notifications.ToList());

        await _dbCommit.Commit(cancellationToken);
        return new BaseResponse(200, "Usuário alterado com sucesso!");
    }
}