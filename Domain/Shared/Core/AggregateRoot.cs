using Flunt.Notifications;

namespace Domain.Shared.Core;

/// <summary>
/// Raiz agregada abstrata.
/// Todas as operações de negócio devem passar por uma raiz agregada.
/// Raízes agregadas são invariavelmente consistentes e geram eventos de domínio.
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<DomainEvent> _uncommittedEvents = [];

    /// <summary>Eventos de domínio não persistidos ainda.</summary>
    public IReadOnlyList<DomainEvent> UncommittedEvents => _uncommittedEvents.AsReadOnly();

    /// <summary>Adiciona um evento à fila de não persistidos.</summary>
    protected void RaiseDomainEvent(DomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _uncommittedEvents.Add(domainEvent);
    }

    /// <summary>Marca eventos como persistidos.</summary>
    public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

    /// <summary>Reconstrói estado agregado a partir de eventos (Event Sourcing).</summary>
    public virtual void ApplyEvent(DomainEvent @event)
    {
        ArgumentNullException.ThrowIfNull(@event);
        // Subclasses devem override e aplicar lógica específica
    }
}
