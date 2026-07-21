using Domain.Pipelines.Aggregates;
using Domain.Pipelines.ValueObjects;
using Domain.Shared.Repositories;

namespace Domain.Pipelines.Repositories;

/// <summary>
/// Repositório do agregado Pipeline.
/// </summary>
public interface IPipelineRepository : IRepository<Pipeline>
{
    Task<Pipeline?> GetByNameAsync(PipelineName name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Pipeline>> GetByStatusAsync(PipelineStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Pipeline>> GetRecentAsync(int count, CancellationToken cancellationToken = default);
}
