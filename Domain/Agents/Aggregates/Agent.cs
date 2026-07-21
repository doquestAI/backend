using Domain.Agents.ValueObjects;
using Domain.Shared.Core;

namespace Domain.Agents.Aggregates;

/// <summary>
/// AGGREGATE ROOT: Agent
/// Representa uma instância de um agente de IA no sistema.
/// Contém configuração, capacidades, status e métricas.
/// Encapsula regras de negócio sobre comportamento de agentes.
/// </summary>
internal sealed class Agent : AggregateRoot
{
    public AgentId AgentId { get; private set; } = null!;
    public AgentName Name { get; private set; } = null!;
    public AgentDescription Description { get; private set; } = null!;
    public AgentRole Role { get; private set; }
    public AgentSystemPrompt SystemPrompt { get; private set; } = null!;
    public AgentStatus Status { get; private set; } = AgentStatus.Idle;
    public AgentCapabilities Capabilities { get; private set; } = null!;
    public AgentMetrics Metrics { get; private set; } = null!;
    public double Temperature { get; private set; }
    public int MaxOutputTokens { get; private set; }
    public bool IsEnabled { get; private set; }

    private Agent() { }

    public static Agent Create(
        AgentName name,
        AgentDescription description,
        AgentRole role,
        AgentSystemPrompt systemPrompt,
        double temperature = 0.7,
        int maxOutputTokens = 2048,
        AgentCapabilities? capabilities = null)
    {
        var agent = new Agent
        {
            Id = Guid.NewGuid(),
            AgentId = AgentId.New(),
            Name = name,
            Description = description,
            Role = role,
            SystemPrompt = systemPrompt,
            Temperature = temperature,
            MaxOutputTokens = maxOutputTokens,
            Capabilities = capabilities ?? AgentCapabilities.Default(),
            Metrics = new AgentMetrics(),
            Status = AgentStatus.Idle,
            IsEnabled = true,
        };

        // Valida value objects
        agent.AddNotificationsFromValueObjects(name, description, systemPrompt);

        if (temperature < 0 || temperature > 2)
            agent.AddNotification(nameof(Temperature), "Temperature must be between 0 and 2");

        if (maxOutputTokens <= 0)
            agent.AddNotification(nameof(MaxOutputTokens), "MaxOutputTokens must be greater than 0");

        if (agent.IsValid)
            agent.RaiseDomainEvent(new AgentCreatedEvent(agent.Id, name.Value, role));

        return agent;
    }

    public void SetStatus(AgentStatus newStatus)
    {
        if (Status == newStatus)
            return;
        Status = newStatus;

        if (newStatus == AgentStatus.Error)
            Metrics.RecordFailure();
    }

    public void RecordInvocation(long inputTokens, long outputTokens, TimeSpan duration)
    {
        Metrics.RecordInvocation(inputTokens, outputTokens, duration);
        RaiseDomainEvent(new AgentInvokedEvent(Id, inputTokens, outputTokens, duration));
    }

    public void Disable()
    {
        IsEnabled = false;
        RaiseDomainEvent(new AgentDisabledEvent(Id, Name.Value));
    }

    public void Enable()
    {
        IsEnabled = true;
        RaiseDomainEvent(new AgentEnabledEvent(Id, Name.Value));
    }

    public void UpdateSystemPrompt(AgentSystemPrompt newPrompt)
    {
        if (SystemPrompt == newPrompt)
            return;
        SystemPrompt = newPrompt;
        RaiseDomainEvent(new AgentSystemPromptUpdatedEvent(Id, Name.Value));
    }

    public void UpdateCapabilities(AgentCapabilities newCapabilities)
    {
        Capabilities = newCapabilities;
        RaiseDomainEvent(new AgentCapabilitiesUpdatedEvent(Id, Name.Value));
    }
}
