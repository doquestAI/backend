namespace Domain.Interfaces.Agents;

internal interface IAgent<in TData, TResponse>
{
    Task<TResponse> RunAsync(TData data, CancellationToken cancellationToken = default);
}
