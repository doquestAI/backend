namespace Domain.Pipelines.ValueObjects;

/// <summary>
/// Métricas de tokens (LLM) de um passo ou pipeline.
/// Imutável.
/// </summary>
public sealed class TokenMetrics
{
    public long InputTokens { get; }
    public long OutputTokens { get; }
    public long TotalTokens => InputTokens + OutputTokens;

    public TokenMetrics(long inputTokens = 0, long outputTokens = 0)
    {
        InputTokens = inputTokens;
        OutputTokens = outputTokens;
    }

    public static TokenMetrics Empty => new();

    public TokenMetrics Add(TokenMetrics other) =>
        new(InputTokens + other.InputTokens, OutputTokens + other.OutputTokens);

    public override string ToString() =>
        $"Tokens {{ Input: {InputTokens}, Output: {OutputTokens}, Total: {TotalTokens} }}";
}
