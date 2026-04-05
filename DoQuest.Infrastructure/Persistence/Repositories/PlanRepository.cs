using DoQuest.Domain.Entities;
using DoQuest.Domain.Enums;
using DoQuest.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DoQuest.Infrastructure.Persistence.Repositories;

public sealed class PlanRepository(DoQuestDbContext context)
    : Repository<Plan>(context), IPlanRepository
{
    public async Task<Plan?> GetByTypeAsync(PlanType type, CancellationToken cancellationToken = default) =>
        await Context.Plans
            .FirstOrDefaultAsync(p => p.Type == type, cancellationToken);
}
