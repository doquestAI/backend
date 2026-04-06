namespace Infrastructure.Options;

internal sealed class OllamaOptions
{
    public const string SectionName = "Ollama";
    public string Endpoint { get; set; } = "http://localhost:11434";
    public string ChatModel { get; set; } = "llama3.2";
    public string EmbeddingModel { get; set; } = "nomic-embed-text";
}
