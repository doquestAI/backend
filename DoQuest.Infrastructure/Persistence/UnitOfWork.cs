using DoQuest.Domain.Repositories;

namespace DoQuest.Infrastructure.Persistence;

public sealed class UnitOfWork(DoQuestDbContext context) : IUnitOfWork
{
    public Task<int> CommitAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
