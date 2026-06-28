using Azure.Monitor.OpenTelemetry.AspNetCore;
using Domain.Configurations;
using Domain.Observability;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Presentation.Common.Observability;

internal static class ObservabilityExtensions
{
    /// <summary>
    /// Configura OpenTelemetry de forma condicional por ambiente:
    ///
    /// <list type="bullet">
    ///   <item><b>Development:</b> exporta spans para o console do terminal.
    ///   Nenhum dado é enviado para o Azure.</item>
    ///   <item><b>Production / Staging:</b> exporta traces, métricas e logs para Azure Monitor
    ///   (Application Insights). A connection string é lida de <c>AzureMonitor:ConnectionString</c>
    ///   no appsettings ou da env var <c>APPLICATIONINSIGHTS_CONNECTION_STRING</c>.</item>
    /// </list>
    /// </summary>
    internal static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
        var settings = builder.Configuration
            .GetSection("ObservabilitySettings")
            .Get<ObservabilitySettings>() ?? new ObservabilitySettings();

        var otel = builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: "Doquest.Backend",
                    serviceVersion: settings.ServiceVersion,
                    autoGenerateServiceInstanceId: true)
                .AddAttributes([
                    KeyValuePair.Create<string, object>(
                        "deployment.environment",
                        builder.Environment.EnvironmentName.ToLowerInvariant())
                ]));

        if (builder.Environment.IsDevelopment())
        {
            // ── Desenvolvimento local: console exporter, sem Azure Monitor ────
            otel.WithTracing(tracing => tracing
                    .AddAspNetCoreInstrumentation(opts => opts.RecordException = true)
                    .AddHttpClientInstrumentation(opts => opts.RecordException = true)
                    .AddEntityFrameworkCoreInstrumentation(opts =>
                    {
                        opts.SetDbStatementForText = true;
                        opts.SetDbStatementForStoredProcedure = true;
                    })
                    .AddSource(TelemetryConstants.Sources.Pipeline)
                    .AddSource(TelemetryConstants.Sources.Agent)
                    .AddConsoleExporter())
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter(TelemetryConstants.Meters.Pipeline)
                    .AddMeter(TelemetryConstants.Meters.Agent));
        }
        else
        {
            // ── Produção / Staging: Azure Monitor (Application Insights) ──────
            // UseAzureMonitor já inclui automaticamente: ASP.NET Core, HttpClient, SQL Client
            otel.UseAzureMonitor(opts => opts.SamplingRatio = settings.SamplingRatio)
                .WithTracing(tracing => tracing
                    .AddEntityFrameworkCoreInstrumentation(opts =>
                    {
                        // Ocultar SQL por padrão em produção para evitar PII nos traces
                        opts.SetDbStatementForText = settings.EnableDetailedSql;
                        opts.SetDbStatementForStoredProcedure = true;
                    })
                    .AddSource(TelemetryConstants.Sources.Pipeline)
                    .AddSource(TelemetryConstants.Sources.Agent))
                .WithMetrics(metrics => metrics
                    .AddRuntimeInstrumentation()
                    .AddMeter(TelemetryConstants.Meters.Pipeline)
                    .AddMeter(TelemetryConstants.Meters.Agent));
        }

        return builder;
    }
}
