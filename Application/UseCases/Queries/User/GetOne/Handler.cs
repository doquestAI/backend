using Domain.Interfaces.Repositories;
using MediatR;
using System.Linq.Expressions;
using UserEntity = Domain.Entities.Core.User;

namespace Application.UseCases.Queries.User.GetOne;

internal class Handler(
    IUserRepository userRepository
) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(
        Request request,
        CancellationToken cancellationToken)
    {
        Expression<Func<UserEntity, bool>> filter = u => u.Id == request.Id && u.IsActiveCredentials == true;

        var user = await userRepository.GetWithParametersAsync(
            filter,
            cancellationToken
        );

        if (user is null)
            return new Response(StatusCode: 404, Message: "Usuário não encontrado");

        return new Response(
            StatusCode: 200,
            Message: "Usuário recuperado com sucesso",
            Notifications: null,
            Id: user.Id,
            FirstName: user.FullName.FirstName ?? string.Empty,
            LastName: user.FullName.LastName,
            Email: user.Email.Address ?? string.Empty,
            Active: user.Active,
            Address: user.Address is not null
                ? new AddressData(
                    Number: user.Address.Number,
                    Street: user.Address.Road,
                    CEP: user.Address.CEP,
                    NeighBordHood: user.Address.NeighBordHood,
                    Complement: user.Address.Complement
                )
                : null
        );
    }
}
