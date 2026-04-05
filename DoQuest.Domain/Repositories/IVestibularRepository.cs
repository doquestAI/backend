using DoQuest.Domain.Entities;

namespace DoQuest.Domain.Repositories;

public interface IVestibularRepository : IRepository<Vestibular>
{
    Task<IReadOnlyList<Vestibular>> GetAllAsync(CancellationToken cancellationToken = default);
}
