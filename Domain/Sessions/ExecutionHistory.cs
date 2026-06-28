using Flunt.Notifications;

namespace Domain.Sessions;

/// <summary>
/// Histórico de passos executados na sessão (auditoria do fluxo do agente).
/// Distinto de <see cref="ConversationMemory"/> — aqui são <i>eventos de execução</i>,
/// não mensagens trocadas com o LLM.
/// </summary>
internal sealed class ExecutionHistory : Notifiable<Notification>
{
    private readonly List<ExecutionEvent> _events = [];

    public IReadOnlyList<ExecutionEvent> Events => _events.AsReadOnly();

    public void Record(string stepName, string? detail = null)
    {
        if (string.IsNullOrWhiteSpace(stepName))
        {
            AddNotification(nameof(stepName), "Execution event step name cannot be empty");
            return;
        }
        _events.Add(new ExecutionEvent(stepName, detail, DateTime.UtcNow));
    }
}

internal sealed record ExecutionEvent(string StepName, string? Detail, DateTime Timestamp);
