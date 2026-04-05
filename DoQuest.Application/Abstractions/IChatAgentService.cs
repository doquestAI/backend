using Microsoft.Extensions.AI;

namespace DoQuest.Application.Abstractions;

public interface IChatAgentService
{
    Task<string> ChatAsync(
        string userMessage,
        string ragContext,
        IEnumerable<ChatMessage> conversationHistory,
        CancellationToken cancellationToken = default);
}
