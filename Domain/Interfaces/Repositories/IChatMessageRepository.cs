using Domain.Entities.Core.Chat;

namespace Domain.Interfaces.Repositories;

internal interface IChatMessageRepository : IBaseRepository<ChatMessage>
{
    Task<List<ChatMessage>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken);
}
