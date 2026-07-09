using Domain.Entities.Abstracts;

namespace Domain.Entities.Core.Chat;

internal class ChatSession : Entity
{
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public Guid ContextUserId { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public ICollection<ChatMessage> Messages { get; private set; } = [];

    private ChatSession() { }

    public ChatSession(Guid contextUserId, string? title = null, string? description = null)
    {
        ContextUserId = contextUserId;
        Title = title;
        Description = description;
    }

    public void EndSession()
    {
        EndedAt = DateTime.UtcNow;
    }
}
