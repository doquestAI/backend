using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Domain.Observability;

/// <summary>
/// Instrumentos OpenTelemetry para a camada de Pipeline.
/// Usa apenas tipos BCL (System.Diagnostics) — sem dependências NuGet no Domain.
/// </summary>
public static class PipelineTelemetry
{
    public static readonly ActivitySource Source =
        new(TelemetryConstants.Sources.Pipeline, "1.0.0");

    private static readonly Meter _meter =
        new(TelemetryConstants.Meters.Pipeline, "1.0.0");

    // ── Contadores de Pipeline ──────────────────────────────────────────
    public static readonly Counter<long> PipelineExecutions =
        _meter.CreateCounter<long>(
            "pipeline.executions",
            unit: "{executions}",
            description: "Número total de execuções de pipelines.");

    public static readonly Counter<long> PipelineFailures =
        _meter.CreateCounter<long>(
            "pipeline.failures",
            unit: "{failures}",
            description: "Número total de pipelines que falharam.");

    public static readonly Histogram<double> PipelineDuration =
        _meter.CreateHistogram<double>(
            "pipeline.duration",
            unit: "ms",
            description: "Duração total de execução de uma pipeline em milissegundos.");

    // ── Contadores de Step ──────────────────────────────────────────────
    public static readonly Counter<long> StepExecutions =
        _meter.CreateCounter<long>(
            "pipeline.step.executions",
            unit: "{executions}",
            description: "Número total de execuções de steps.");

    public static readonly Counter<long> StepFailures =
        _meter.CreateCounter<long>(
            "pipeline.step.failures",
            unit: "{failures}",
            description: "Número total de steps que falharam.");

    public static readonly Histogram<double> StepDuration =
        _meter.CreateHistogram<double>(
            "pipeline.step.duration",
            unit: "ms",
            description: "Duração de execução de um step em milissegundos.");
}
