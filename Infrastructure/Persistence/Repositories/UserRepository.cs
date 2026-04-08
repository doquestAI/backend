using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(CoreDbContext context)
    : BaseRepository<User>(context), IUserRepository
{
    public async Task<User?> GetByFirebaseUidAsync(string firebaseUid, CancellationToken cancellationToken = default) =>
        await context.Users
            .Include(u => u.Plan)
            .FirstOrDefaultAsync(u => u.FirebaseUid.Value == firebaseUid, cancellationToken);
}