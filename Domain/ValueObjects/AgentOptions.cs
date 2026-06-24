namespace Domain.ValueObjects;

public sealed class AgentOptions
{
    public string ModelId { get; set; } = "gpt-4o-mini";
    public float Temperature { get; set; } = 0.7f;
    public int MaxTokens { get; set; } = 2048;
    public int MaxHistory { get; set; } = 20;
    public bool EnableMemory { get; set; } = false;
    public bool EnableRag { get; set; } = false;
    public int RagTopK { get; set; } = 5;
    public float RagThreshold { get; set; } = 0.75f;
    public string? SystemPromptKey { get; set; }
}