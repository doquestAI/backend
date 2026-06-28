using Flunt.Notifications;

namespace Domain.Sessions;

/// <summary>
/// Histórico de mensagens da sessão atual (short-term memory).
/// O Agent consulta isto para saber o que foi dito antes.
/// </summary>
public sealed class ConversationMemory : Notifiable<Notification>
{
    private readonly List<MemoryEntry> _entries = [];

    public IReadOnlyList<MemoryEntry> Entries => _entries.AsReadOnly();
    public int TurnCount => _entries.Count(e => e.Role == MemoryRole.User);

    public void AddEntry(MemoryRole role, string content, string? name = null)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            AddNotification(nameof(content), "Memory entry content cannot be empty");
            return;
        }
        _entries.Add(new MemoryEntry(role, content, name, DateTime.UtcNow));
    }

    public IEnumerable<MemoryEntry> GetLastN(int n) => _entries.TakeLast(n);

    public void Clear() => _entries.Clear();
}
