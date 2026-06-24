namespace Domain.Interfaces.Agents;

/// <summary>
/// Contrato base para todos os agentes do sistema.
///   TIn  = tipo da entrada (string, command, DTO, etc.)
///   TOut = tipo da saída  (string, result, DTO, etc.)
///
/// <paramref name="sessionKey"/> é opcional — quando informado, o agente
/// hidrata e persiste o histórico da conversa via cache distribuído.
/// </summary>
public interface IAgent<TIn, TOut>
{
    Task<TOut> RunAsync(
        TIn input,
        string? sessionKey = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Contrato para agentes que suportam resposta em streaming (texto incremental).
/// Útil em UIs de chat e geração de conteúdo longo.
/// </summary>
public interface IStreamingAgent<TIn>
{
    IAsyncEnumerable<string> RunStreamingAsync(
        TIn input,
        string? sessionKey = null,
        CancellationToken cancellationToken = default);
}
