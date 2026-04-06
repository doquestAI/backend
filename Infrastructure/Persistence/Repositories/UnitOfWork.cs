using Domain.Interfaces.Repositories;

namespace Infrastructure.Persistence.Repositories;

internal sealed class UnitOfWork(CoreDbContext context) : IUnitOfWork
{
    public Task<int> CommitAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
