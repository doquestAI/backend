using DoQuest.Domain.Entities;

namespace DoQuest.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByFirebaseUidAsync(string firebaseUid, CancellationToken cancellationToken = default);
}
