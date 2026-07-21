namespace Domain.Sessions.ValueObjects;

/// <summary>
/// Papel de uma entrada de memória de conversa.
/// Define quem falou: usuário, agente, ou sistema.
/// </summary>
public enum MemoryRole
{
    /// <summary>Entrada do usuário/cliente.</summary>
    User = 0,

    /// <summary>Resposta do agente.</summary>
    Agent = 1,

    /// <summary>Mensagem do sistema (instruções, contexto).</summary>
    System = 2,
}
