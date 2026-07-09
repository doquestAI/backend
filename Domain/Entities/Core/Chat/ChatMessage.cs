using Domain.Entities.Abstracts;

namespace Domain.Entities.Core.Chat;

internal class ChatMessage : Entity
{
    public Guid ChatSessionId { get; private set; }
    public string Role { get; private set; } = null!;
    public string Content { get; private set; } = null!;

    private ChatMessage() { }

    public ChatMessage(Guid chatSessionId, string role, string content)
    {
        ChatSessionId = chatSessionId;
        Role = role;
        Content = content;
    }
}
