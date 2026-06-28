using Flunt.Notifications;

namespace Domain.Pipelines;

/// <summary>
/// Resultado da execução de um único <see cref="PipelineStep"/>.
/// Falhas viram notificações (não exceptions). Métricas sempre presentes.
/// </summary>
public sealed class StepResult : Notifiable<Notification>
{
    public object? Value { get; }
    public StepMetrics Metrics { get; }

    private StepResult(object? value, StepMetrics metrics)
    {
        Value = value;
        Metrics = metrics;
    }

    private StepResult(IReadOnlyCollection<Notification> notifications, StepMetrics metrics)
    {
        AddNotifications(notifications);
        Metrics = metrics;
    }

    public static StepResult Success(object? value, StepMetrics metrics) =>
        new(value, metrics);

    public static StepResult Fail(string property, string message, StepMetrics metrics) =>
        new([new Notification(property, message)], metrics);

    public static StepResult Fail(IReadOnlyCollection<Notification> notifications, StepMetrics metrics) =>
        new(notifications, metrics);
}
