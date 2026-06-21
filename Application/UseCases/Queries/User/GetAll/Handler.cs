using Domain.Interfaces.Repositories;
using MediatR;
using System.Linq.Expressions;
using UserEntity = Domain.Entities.Core.User;

namespace Application.UseCases.Queries.User.GetAll;

internal class Handler(
    IUserRepository userRepository
) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(
        Request request,
        CancellationToken cancellationToken)
    {
        Expression<Func<UserEntity, bool>>? filter = u => u.IsActiveCredentials == true;

        if (!string.IsNullOrWhiteSpace(request.SearchTerm) || request.IsActive.HasValue)
        {
            filter = u =>
                (string.IsNullOrWhiteSpace(request.SearchTerm) ||
                 u.FullName.FirstName!.Contains(request.SearchTerm) ||
                 u.FullName.LastName!.Contains(request.SearchTerm) ||
                 u.Email.Address!.Contains(request.SearchTerm)) &&
                (!request.IsActive.HasValue || u.Active == request.IsActive.Value);
        }

        var pagedResult = await userRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filter,
            cancellationToken
        );

        var items = pagedResult.Items.Select(u => new UserData(
            Id: u.Id,
            FirstName: u.FullName.FirstName ?? string.Empty,
            LastName: u.FullName.LastName,
            Email: u.Email.Address ?? string.Empty,
            Active: u.Active,
            IsActiveCredentials: u.IsActiveCredentials
        )).ToList();

        return new Response(
            StatusCode: 200,
            Message: "Usuários recuperados com sucesso",
            Notifications: null,
            Items: items,
            TotalCount: pagedResult.TotalCount,
            PageNumber: pagedResult.PageNumber,
            PageSize: pagedResult.PageSize,
            TotalPages: pagedResult.TotalPages,
            HasPreviousPage: pagedResult.HasPreviousPage,
            HasNextPage: pagedResult.HasNextPage
        );
    }
}
