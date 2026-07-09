using Domain.Entities.Core.Chat;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal class ChatSessionRepository(CoreDbContext context)
    : BaseRepository<ChatSession>(context), IChatSessionRepository
{
    public async Task<ChatSession?> GetByIdWithMessagesAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        return await context.Set<ChatSession>()
            .AsNoTracking()
            .Include(s => s.Messages)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.DeletedDate == null, cancellationToken);
    }

    public async Task<List<ChatSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await context.Set<ChatSession>()
            .AsNoTracking()
            .Where(s => s.ContextUserId == userId && s.DeletedDate == null)
            .OrderByDescending(s => s.CreatedDate)
            .ToListAsync(cancellationToken);
    }
}
