namespace Domain.Pipelines;

/// <summary>
/// Contagem de tokens consumidos em uma invocação ao LLM.
/// Valor obtido do <c>UsageDetails</c> retornado pelo MAF/MEAI.
/// </summary>
internal sealed class TokenUsage
{
    public long InputTokens { get; }
    public long OutputTokens { get; }
    public long TotalTokens => InputTokens + OutputTokens;

    public TokenUsage(long inputTokens, long outputTokens)
    {
        InputTokens = inputTokens;
        OutputTokens = outputTokens;
    }

    internal static TokenUsage Empty => new(0, 0);

    public TokenUsage Add(TokenUsage other) =>
        new(InputTokens + other.InputTokens, OutputTokens + other.OutputTokens);

    public override string ToString() =>
        $"in={InputTokens} out={OutputTokens} total={TotalTokens}";
}
