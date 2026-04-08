using Domain.Entities;

namespace Domain.Interfaces.Repositories;

internal interface IChatSessionRepository : IBaseRepository<ChatSession>
{
    Task<ChatSession?> GetWithMessagesAsync(Guid sessionId, int messageLimit, CancellationToken ct = default);
    Task<IReadOnlyList<ChatSession>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken ct = default);
    Task AddMessagesAsync(IEnumerable<ChatMessage> messages, CancellationToken ct = default);
}