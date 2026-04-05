using DoQuest.Domain.Entities;
using DoQuest.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DoQuest.Infrastructure.Persistence.Repositories;

public sealed class VestibularRepository(DoQuestDbContext context)
    : Repository<Vestibular>(context), IVestibularRepository
{
    public async Task<IReadOnlyList<Vestibular>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await Context.Vestibulares
            .AsNoTracking()
            .OrderBy(v => v.Name)
            .ToListAsync(cancellationToken);
}
