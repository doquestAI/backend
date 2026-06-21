using Domain.Entities;
using Domain.Entities.Core;
using System;

namespace Domain.Interfaces.Repositories;

internal interface IUserRepository : IBaseRepository<User>
{
    Task<User?> Authenticate(User user, CancellationToken cancellationToken);
    Task<User?> ActivateUserAsync(string email, string token, CancellationToken cancellationToken);
    Task<User?> GetByEmail(string email, CancellationToken cancellationToken);
    Task<User?> GetByIdWithRolesAsync(
        Guid userId,
        CancellationToken cancellationToken);
}