namespace Domain.Configurations;

/// <summary>
/// Configurações de observabilidade. A connection string do Azure Monitor é lida
/// diretamente pela chave <c>AzureMonitor:ConnectionString</c> (padrão do SDK).
/// </summary>
public sealed class ObservabilitySettings
{
    /// <summary>Proporção de traces amostrados (1.0 = 100 %). Reduza em produção de alto volume.</summary>
    public float SamplingRatio { get; init; } = 1.0f;

    /// <summary>Versão do serviço exibida no Application Insights e no OTel Resource.</summary>
    public string ServiceVersion { get; init; } = "1.0.0";

    /// <summary>
    /// Quando true, inclui o texto SQL completo nos spans do EF Core.
    /// Desabilite em produção para evitar PII nos traces.
    /// </summary>
    public bool EnableDetailedSql { get; init; } = false;
}
