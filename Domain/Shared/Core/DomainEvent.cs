namespace Domain.Shared.Core;

/// <summary>
/// Evento de domínio abstrato.
/// Eventos representam algo que aconteceu no passado.
/// Imutáveis, com timestamp, para reconstrução de estado.
/// </summary>
public abstract record DomainEvent(Guid AggregateId, DateTime OccurredAt)
{
    /// <summary>ID único do evento (para idempotência).</summary>
    public Guid EventId { get; init; } = Guid.NewGuid();

    /// <summary>Versão do agregado quando evento foi gerado.</summary>
    public int AggregateVersion { get; init; }

    protected DomainEvent(Guid aggregateId) : this(aggregateId, DateTime.UtcNow) { }
}
