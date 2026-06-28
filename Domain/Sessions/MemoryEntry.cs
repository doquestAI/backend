namespace Domain.Sessions;

/// <summary>Uma mensagem/evento do histórico de conversa.</summary>
public sealed record MemoryEntry(
    MemoryRole Role,
    string Content,
    string? Name,
    DateTime Timestamp);
