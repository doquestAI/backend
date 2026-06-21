using Domain.Configurations;
using Domain.Entities.Core;
using Domain.Interfaces.Repositories;
using Infrastructure.Cache;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories;

internal class UserRepository(CoreDbContext context, IOptions<AppSettings> appSettings)
    : BaseRepository<User>(context), IUserRepository
{
    private readonly string _encryptionKey = appSettings.Value.EncryptionKey;
    public async Task<User?> Authenticate(User user, CancellationToken cancellationToken)
    {
        var userFromDb = await context.Set<User>()
            .AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .NoCache()
            .FirstOrDefaultAsync(u => u.Email.Address == user.Email.Address && u.Active && u.IsActiveCredentials, cancellationToken);
        return userFromDb;
    }

    public async Task<User?> ActivateUserAsync(string email, string token, CancellationToken cancellationToken)
    {
        var users = await context.Set<User>()
             .NoCache()
             .Where(x => !x.Active && x.Email.Address!.Equals(email) && x.TokenActivate != null)
             .ToListAsync(cancellationToken);

        var user = users.FirstOrDefault(x => x.TokenActivate!.Validate(token, _encryptionKey));

        if (user is not null)
        {
            user.AssignActivate(true);
            Update(user);
        }
        return user;
    }

    public async Task<User?> GetByEmail(string email, CancellationToken cancellationToken) =>
        await context.Set<User>()
            .AsNoTracking()
            .NoCache()
            .FirstOrDefaultAsync(x => x.Email.Address!.Equals(email), cancellationToken);

    public async Task<User?> GetByIdWithRolesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }
}