using Domain.Entities;

namespace Domain.Interfaces.Repositories;

internal interface IVestibularRepository : IBaseRepository<Vestibular>
{
    Task<IReadOnlyList<Vestibular>> GetAllAsync(CancellationToken cancellationToken = default);
}
