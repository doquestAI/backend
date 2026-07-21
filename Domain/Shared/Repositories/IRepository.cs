using Domain.Shared.Core;

namespace Domain.Shared.Repositories;

/// <summary>
/// Interface genérica de repositório.
/// Cada Bounded Context implementa seu próprio repositório.
/// Repositories trabalham com Aggregates, nunca com Entities internas.
/// </summary>
public interface IRepository<T> where T : AggregateRoot
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ListAsync(Specification<T> spec, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Specification<T> spec, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Specification<T> spec, CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}
