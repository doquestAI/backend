namespace Domain.Agents;

/// <summary>Papel semântico do Agent dentro de um fluxo multiagente.</summary>
public enum AgentRole
{
    Helper = 0,
    Specialist = 1,
    Orchestrator = 2,
    Assistant = 3,
    Evaluator = 4,
    Generator = 5,
}
