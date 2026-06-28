namespace AI.Common;

/// <summary>
/// Carrega flags de invocação por chamada via AsyncLocal, permitindo que provedores singleton
/// (como RagContextProvider) leiam opções definidas em AgentInvocationInput sem serem
/// reconstruídos a cada request.
/// </summary>
internal static class AgentInvocationScope
{
    private static readonly AsyncLocal<bool> _useRag = new();

    internal static bool UseRag
    {
        get => _useRag.Value;
        set => _useRag.Value = value;
    }
}
