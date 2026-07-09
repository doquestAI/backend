using Domain.Entities.Core.Chat;
using Domain.Interfaces.Repositories;
using DomainChatMessage = Domain.Entities.Core.Chat.ChatMessage;
using AIChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace Infrastructure.Services;

internal class ChatHistoryService(
    IChatMessageRepository chatMessageRepository)
{
    public async Task<IList<AIChatMessage>> LoadHistoryAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var messages = await chatMessageRepository.GetBySessionIdAsync(sessionId, cancellationToken);
        return messages
            .Select(m => new AIChatMessage(new Microsoft.Extensions.AI.ChatRole(m.Role), m.Content))
            .ToList<AIChatMessage>();
    }

    public async Task<DomainChatMessage> AddMessageAsync(
        Guid sessionId,
        string role,
        string content,
        CancellationToken cancellationToken = default)
    {
        var message = new DomainChatMessage(sessionId, role.ToLowerInvariant(), content);
        await chatMessageRepository.CreateAsync(message, cancellationToken);
        return message;
    }

    public async Task PersistHistoryAsync(Guid sessionId, IEnumerable<AIChatMessage> messages, CancellationToken cancellationToken = default)
    {
        var domainMessages = messages
            .Select(m => new DomainChatMessage(sessionId, m.Role.Value.ToLowerInvariant(), m.Text ?? ""))
            .ToList();

        foreach (var msg in domainMessages)
        {
            await chatMessageRepository.CreateAsync(msg, cancellationToken);
        }
    }
}
