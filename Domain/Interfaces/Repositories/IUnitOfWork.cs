namespace Domain.Interfaces.Repositories;

internal interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
