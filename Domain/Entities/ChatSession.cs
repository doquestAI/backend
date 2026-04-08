using Domain.Entities.Abstracts;
using Domain.Enums;
using Flunt.Notifications;

namespace Domain.Entities;

internal class ChatSession : Entity
{
    private ChatSession() { }

    private readonly List<ChatMessage> _messages = [];

    public Guid UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public Guid? VestibularId { get; private set; }
    public string? SerializedAgentSession { get; private set; }
    public DateTime? LastMessageAt { get; private set; }

    // Navigation properties (set by EF Core)
    public User User { get; private set; } = null!;
    public Vestibular? Vestibular { get; private set; }
    public IReadOnlyCollection<ChatMessage> Messages => _messages.AsReadOnly();

    public static ChatSession Create(Guid userId, string title, Guid? vestibularId = null)
    {
        var session = new ChatSession();

        if (userId == Guid.Empty)
            session.AddNotification(new Notification(nameof(UserId), "UserId é obrigatório"));

        if (string.IsNullOrWhiteSpace(title))
            session.AddNotification(new Notification(nameof(Title), "Título da sessão é obrigatório"));

        if (session.IsValid)
        {
            session.UserId = userId;
            session.Title = title.Trim();
            session.VestibularId = vestibularId;
        }

        return session;
    }

    public void AddMessage(ChatMessageRole role, string content)
    {
        var message = ChatMessage.Create(Id, role, content);

        if (!message.IsValid)
        {
            AddNotifications(message);
            return;
        }

        _messages.Add(message);
        LastMessageAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return;
        Title = title.Trim();
        MarkAsUpdated();
    }

    public void StoreAgentSession(string serializedSession)
    {
        SerializedAgentSession = serializedSession;
        MarkAsUpdated();
    }
}