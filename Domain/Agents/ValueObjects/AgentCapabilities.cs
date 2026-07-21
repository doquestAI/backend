namespace Domain.Agents.ValueObjects;

/// <summary>
/// Capacidades de um agente: plugins, MCPs, context providers.
/// Value Object imutável que define o que o agente pode fazer.
/// </summary>
public sealed class AgentCapabilities
{
    public IReadOnlyList<string> Plugins { get; }
    public IReadOnlyList<string> McpConnections { get; }
    public bool SupportsStreaming { get; }
    public bool SupportsRag { get; }

    public AgentCapabilities(
        IReadOnlyList<string>? plugins = null,
        IReadOnlyList<string>? mcps = null,
        bool supportsStreaming = false,
        bool supportsRag = false)
    {
        Plugins = plugins ?? [];
        McpConnections = mcps ?? [];
        SupportsStreaming = supportsStreaming;
        SupportsRag = supportsRag;
    }

    public static AgentCapabilities Default() => new();
    public static AgentCapabilities WithStreaming() => new(supportsStreaming: true);
    public static AgentCapabilities WithRag() => new(supportsRag: true);
}
