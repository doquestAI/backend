using DoQuest.Domain.Entities;
using DoQuest.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DoQuest.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(DoQuestDbContext context)
    : Repository<User>(context), IUserRepository
{
    public async Task<User?> GetByFirebaseUidAsync(string firebaseUid, CancellationToken cancellationToken = default) =>
        await Context.Users
            .Include(u => u.Plan)
            .FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid, cancellationToken);
}
