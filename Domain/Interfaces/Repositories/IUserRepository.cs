
using Domain.Entities;

namespace Domain.Interfaces.Repositories;

internal interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByFirebaseUidAsync(string firebaseUid, CancellationToken cancellationToken = default);
}
