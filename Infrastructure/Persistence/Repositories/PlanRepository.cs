using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class PlanRepository(CoreDbContext context)
    : BaseRepository<Plan>(context), IPlanRepository
{
    public async Task<Plan?> GetByTypeAsync(PlanType type, CancellationToken cancellationToken = default) =>
        await context.Plans
            .FirstOrDefaultAsync(p => p.Type == type, cancellationToken);
}