namespace Domain.ValueObjects;

public sealed record PromptTemplate(string Raw)
{
    public string Render(IReadOnlyDictionary<string, string> variables)
    {
        var result = Raw;
        foreach (var (key, value) in variables)
            result = result.Replace($"{{{{{key}}}}}", value, StringComparison.OrdinalIgnoreCase);
        return result;
    }
}
