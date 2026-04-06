using Microsoft.Extensions.AI;

namespace Domain.Interfaces.Services;

internal interface IChatAgentService
{
    Task<string> ChatAsync(
        string userMessage,
        string ragContext,
        IEnumerable<ChatMessage> conversationHistory,
        CancellationToken cancellationToken = default);
}
