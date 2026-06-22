namespace Domain.ValueObjects;

public sealed record AgentResult<T>(
    T Value,
    bool Success,
    string? Error      = null,
    int TokensUsed     = 0,
    TimeSpan Elapsed   = default
)
{
    public static AgentResult<T> Ok(T value, int tokens = 0, TimeSpan elapsed = default)
        => new(value, true, null, tokens, elapsed);

    public static AgentResult<T> Fail(string error)
        => new(default!, false, error);
}
