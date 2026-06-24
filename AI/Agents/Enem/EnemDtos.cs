namespace AI.Agents.Enem;

public sealed record ExplainRequest(string Topic, string Area);

public sealed record QuestionRequest(string Topic, string Area, string Difficulty = "Médio");

public sealed record EnemQuestion(
    string Statement,
    IReadOnlyList<string> Options,
    string CorrectKey,
    string Explanation,
    string Area);

public sealed record FeedbackRequest(
    string Question,
    string StudentAnswer,
    string CorrectAnswer,
    string Area = "Geral");

public sealed record FeedbackResult(bool IsCorrect, string Explanation, float Score);