namespace Domain.Interfaces.Agents;

/// <summary>
/// Contrato base para todos os agentes do sistema.
/// TIn  = tipo da entrada (string, command, DTO, etc.)
/// TOut = tipo da saída  (string, result, DTO, etc.)
/// </summary>
public interface IAgent<TIn, TOut>
{
    Task<TOut> RunAsync(TIn input, CancellationToken cancellationToken = default);
}
