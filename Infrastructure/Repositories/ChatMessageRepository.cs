using Domain.Entities.Core.Chat;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal class ChatMessageRepository(CoreDbContext context)
    : BaseRepository<ChatMessage>(context), IChatMessageRepository
{
    public async Task<List<ChatMessage>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        return await context.Set<ChatMessage>()
            .AsNoTracking()
            .Where(m => m.ChatSessionId == sessionId && m.DeletedDate == null)
            .OrderBy(m => m.CreatedDate)
            .ToListAsync(cancellationToken);
    }
}
