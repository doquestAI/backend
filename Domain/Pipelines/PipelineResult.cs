using Flunt.Notifications;

namespace Domain.Pipelines;

/// <summary>
/// Resultado tipado da execução de uma <see cref="Pipeline{TIn,TOut}"/>.
/// Acumula notificações em vez de lançar — quem chamou decide o que fazer.
/// </summary>
internal sealed class PipelineResult<TValue> : Notifiable<Notification>
{
    public TValue? Value { get; }
    public PipelineMetrics Metrics { get; }
    public PipelineId PipelineId { get; }

    private PipelineResult(TValue? value, PipelineMetrics metrics, PipelineId pipelineId)
    {
        Value = value;
        Metrics = metrics;
        PipelineId = pipelineId;
    }

    private PipelineResult(
        IReadOnlyCollection<Notification> notifications,
        PipelineMetrics metrics,
        PipelineId pipelineId)
    {
        AddNotifications(notifications);
        Metrics = metrics;
        PipelineId = pipelineId;
    }

    internal static PipelineResult<TValue> Success(TValue value, PipelineMetrics metrics, PipelineId id) =>
        new(value, metrics, id);

    internal static PipelineResult<TValue> Fail(
        IReadOnlyCollection<Notification> notifications, PipelineMetrics metrics, PipelineId id) =>
        new(notifications, metrics, id);

    internal static PipelineResult<TValue> Fail(string property, string message, PipelineMetrics metrics, PipelineId id) =>
        new([new Notification(property, message)], metrics, id);
}

/// <summary>Versão não-tipada (interna ao Pipeline base não-genérico).</summary>
internal sealed class PipelineResult : Notifiable<Notification>
{
    public object? Value { get; }
    public PipelineMetrics Metrics { get; }
    public PipelineId PipelineId { get; }

    private PipelineResult(object? value, PipelineMetrics metrics, PipelineId id)
    {
        Value = value;
        Metrics = metrics;
        PipelineId = id;
    }

    private PipelineResult(
        IReadOnlyCollection<Notification> notifications,
        PipelineMetrics metrics,
        PipelineId id)
    {
        AddNotifications(notifications);
        Metrics = metrics;
        PipelineId = id;
    }

    internal static PipelineResult Success(object? value, PipelineMetrics metrics, PipelineId id) =>
        new(value, metrics, id);

    internal static PipelineResult Fail(
        IReadOnlyCollection<Notification> notifications, PipelineMetrics metrics, PipelineId id) =>
        new(notifications, metrics, id);
}
