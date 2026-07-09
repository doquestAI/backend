using Domain.Entities.Core.Chat;

namespace Domain.Interfaces.Repositories;

internal interface IChatSessionRepository : IBaseRepository<ChatSession>
{
    Task<ChatSession?> GetByIdWithMessagesAsync(Guid sessionId, CancellationToken cancellationToken);
    Task<List<ChatSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
