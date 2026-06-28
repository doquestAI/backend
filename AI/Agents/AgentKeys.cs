namespace AI.Agents;

/// <summary>Chaves usadas no DI (Keyed Services) para resolver cada AIAgent configurado.</summary>
internal static class AgentKeys
{
    public const string Helper = "enem-helper";
    public const string Explainer = "enem-explainer";
    public const string QuestionGenerator = "enem-question-generator";
    public const string Feedback = "enem-feedback";
}
