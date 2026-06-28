using Domain.Observability;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace AI.Observability;

/// <summary>
/// Instrumentos OpenTelemetry para a camada de Agents (MAF/IChatClient).
/// Segue OTel GenAI Semantic Conventions para compatibilidade com Azure Monitor dashboards.
/// </summary>
internal static class AgentTelemetry
{
    internal static readonly ActivitySource Source =
        new(TelemetryConstants.Sources.Agent, "1.0.0");

    private static readonly Meter _meter =
        new(TelemetryConstants.Meters.Agent, "1.0.0");

    internal static readonly Counter<long> Invocations =
        _meter.CreateCounter<long>(
            "gen_ai.agent.invocations",
            unit: "{invocations}",
            description: "Total de invocações de agents.");

    internal static readonly Counter<long> Failures =
        _meter.CreateCounter<long>(
            "gen_ai.agent.failures",
            unit: "{failures}",
            description: "Total de invocações de agents que falharam.");

    internal static readonly Counter<long> InputTokens =
        _meter.CreateCounter<long>(
            "gen_ai.usage.input_tokens",
            unit: "{tokens}",
            description: "Total de tokens de entrada consumidos pelos agents.");

    internal static readonly Counter<long> OutputTokens =
        _meter.CreateCounter<long>(
            "gen_ai.usage.output_tokens",
            unit: "{tokens}",
            description: "Total de tokens de saída produzidos pelos agents.");

    internal static readonly Histogram<double> InvocationDuration =
        _meter.CreateHistogram<double>(
            "gen_ai.agent.duration",
            unit: "ms",
            description: "Duração de invocação de agent em milissegundos.");
}
