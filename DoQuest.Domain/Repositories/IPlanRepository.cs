using DoQuest.Domain.Entities;

namespace DoQuest.Domain.Repositories;

public interface IPlanRepository : IRepository<Plan>
{
    Task<Plan?> GetByTypeAsync(DoQuest.Domain.Enums.PlanType type, CancellationToken cancellationToken = default);
}
