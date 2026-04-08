using Domain.Entities.Abstracts;
using Domain.Enums;
using Flunt.Notifications;

namespace Domain.Entities;

internal class ChatMessage : Entity
{
    private ChatMessage() { }

    public Guid ChatSessionId { get; private set; }
    public ChatMessageRole Role { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public int TokenCount { get; private set; }

    // Navigation property (set by EF Core)
    public ChatSession ChatSession { get; private set; } = null!;

    public static ChatMessage Create(Guid chatSessionId, ChatMessageRole role, string content, int tokenCount = 0)
    {
        var message = new ChatMessage();

        if (chatSessionId == Guid.Empty)
            message.AddNotification(new Notification(nameof(ChatSessionId), "ChatSessionId é obrigatório"));

        if (string.IsNullOrWhiteSpace(content))
            message.AddNotification(new Notification(nameof(Content), "Conteúdo da mensagem é obrigatório"));

        if (message.IsValid)
        {
            message.ChatSessionId = chatSessionId;
            message.Role = role;
            message.Content = content.Trim();
            message.TokenCount = tokenCount;
        }

        return message;
    }
}