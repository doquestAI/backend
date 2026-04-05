using DoQuest.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DoQuest.Infrastructure.Persistence.Repositories;

public abstract class Repository<T>(DoQuestDbContext context) : IRepository<T> where T : class
{
    protected readonly DoQuestDbContext Context = context;

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await Context.Set<T>().FindAsync([id], cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await Context.Set<T>().AddAsync(entity, cancellationToken);

    public void Update(T entity) =>
        Context.Set<T>().Update(entity);

    public void Remove(T entity) =>
        Context.Set<T>().Remove(entity);
}
