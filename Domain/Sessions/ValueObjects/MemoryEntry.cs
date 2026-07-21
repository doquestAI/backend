using Domain.Shared.Core;

namespace Domain.Sessions.ValueObjects;

/// <summary>
/// Uma entrada no histórico de conversa.
/// Imutável, com timestamp, nunca deletada (auditória).
/// </summary>
public sealed class MemoryEntry : ValueObject
{
    public MemoryRole Role { get; }
    public string Content { get; }
    public string? Name { get; }
    public DateTime CreatedAt { get; }

    public MemoryEntry(MemoryRole role, string content, string? name = null, DateTime? createdAt = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Memory entry content cannot be empty");

        Role = role;
        Content = content;
        Name = name;
        CreatedAt = createdAt ?? DateTime.UtcNow;
    }

    public override bool Equals(object? obj) =>
        obj is MemoryEntry other &&
        Role == other.Role &&
        Content == other.Content &&
        CreatedAt == other.CreatedAt;

    public override int GetHashCode() =>
        HashCode.Combine(Role, Content, CreatedAt);

    public override string ToString() =>
        $"[{Role}] {Name ?? "unknown"}: {Content[..Math.Min(100, Content.Length)]}...";
}
