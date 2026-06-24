namespace Domain.Agents.Enem;

/// <summary>Solicita uma explicação detalhada de um tópico ENEM.</summary>
public sealed record ExplainRequest(string Topic, string Area);

/// <summary>Solicita a geração de uma questão estilo ENEM.</summary>
public sealed record QuestionRequest(string Topic, string Area, string Difficulty = "Médio");

/// <summary>Questão ENEM completa (saída do QuestionAgent).</summary>
public sealed record EnemQuestion(
    string Statement,
    IReadOnlyList<string> Options,
    string CorrectKey,
    string Explanation,
    string Area);

/// <summary>Solicita avaliação da resposta do candidato.</summary>
public sealed record FeedbackRequest(
    string Question,
    string StudentAnswer,
    string CorrectAnswer,
    string Area = "Geral");

/// <summary>Resultado da avaliação (saída do FeedbackAgent).</summary>
public sealed record FeedbackResult(bool IsCorrect, string Explanation, float Score);
