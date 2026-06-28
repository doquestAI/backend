namespace Domain.Sessions;

/// <summary>Uma mensagem/evento do histórico de conversa.</summary>
internal sealed record MemoryEntry(
    MemoryRole Role,
    string Content,
    string? Name,
    DateTime Timestamp);
