using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using System;


namespace Infrastructure.Repositories;

internal class DbCommit(CoreDbContext context) : IDbCommit
{
    public async Task Commit(CancellationToken cancellationToken)
        => await context.SaveChangesAsync(cancellationToken);
}