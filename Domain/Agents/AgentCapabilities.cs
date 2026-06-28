namespace Domain.Agents;

/// <summary>
/// Conjunto de capabilities macro habilitadas para um Agent.
/// Representa o "o que o Agent pode fazer" no nível semântico (não a lista de tools).
/// </summary>
public sealed class AgentCapabilities
{
    public bool SupportsStreaming { get; init; }
    public bool SupportsFunctionCalling { get; init; }
    public bool SupportsRag { get; init; }
    public bool SupportsLongTermMemory { get; init; }
    public bool SupportsMcp { get; init; }

    public static AgentCapabilities Default() => new()
    {
        SupportsStreaming = true,
        SupportsFunctionCalling = true,
        SupportsRag = false,
        SupportsLongTermMemory = false,
        SupportsMcp = false,
    };
}
