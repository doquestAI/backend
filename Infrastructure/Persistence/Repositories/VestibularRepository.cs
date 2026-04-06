using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class VestibularRepository(CoreDbContext context)
    : BaseRepository<Vestibular>(context), IVestibularRepository
{
    public async Task<IReadOnlyList<Vestibular>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Vestibulares
            .AsNoTracking()
            .OrderBy(v => v.Name.Value)
            .ToListAsync(cancellationToken);
}
