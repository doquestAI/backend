namespace Domain.Agents.ValueObjects;

/// <summary>
/// Status da instância de um agente durante execução.
/// Lifecycle: Idle → Running → Done (ou Error).
/// </summary>
public enum AgentStatus
{
    /// <summary>Aguardando invocação.</summary>
    Idle = 0,

    /// <summary>Processando requisição.</summary>
    Running = 1,

    /// <summary>Finalizou (sucesso ou erro).</summary>
    Done = 2,

    /// <summary>Erro durante execução.</summary>
    Error = 3,
}
