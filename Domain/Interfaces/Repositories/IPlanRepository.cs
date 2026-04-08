using Domain.Entities;

namespace Domain.Interfaces.Repositories;

internal interface IPlanRepository : IBaseRepository<Plan>
{
    Task<Plan?> GetByTypeAsync(Domain.Enums.PlanType type, CancellationToken cancellationToken = default);
}