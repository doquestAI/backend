namespace Domain.Agents.ValueObjects;

/// <summary>
/// Papel semântico do agente dentro de um fluxo multiagente.
/// Define responsabilidade e comportamento esperado.
/// </summary>
public enum AgentRole
{
    /// <summary>Ajudante - responde dúvidas, texto livre, suporta streaming.</summary>
    Helper = 0,

    /// <summary>Especialista - executa tarefas de domínio com foco (gerar, explicar, corrigir).</summary>
    Specialist = 1,

    /// <summary>Orquestrador - coordena múltiplos agentes (composição de pipelines).</summary>
    Orchestrator = 2,

    /// <summary>Assistente genérico.</summary>
    Assistant = 3,

    /// <summary>Avaliador - analisa, classifica, valida respostas.</summary>
    Evaluator = 4,

    /// <summary>Gerador - cria artefatos (questões, textos, imagens).</summary>
    Generator = 5,
}
