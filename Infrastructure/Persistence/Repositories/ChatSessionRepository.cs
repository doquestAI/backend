using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class ChatSessionRepository(CoreDbContext context)
    : BaseRepository<ChatSession>(context), IChatSessionRepository
{
    public async Task<ChatSession?> GetWithMessagesAsync(Guid sessionId, int messageLimit, CancellationToken ct = default)
    {
        var query = context.Set<ChatSession>()
            .Where(s => s.Id == sessionId && s.DeletedAt == null);

        if (messageLimit > 0)
        {
            // Load session with the last N messages ordered by creation time
            return await query
                .Select(s => new
                {
                    Session = s,
                    Messages = s.Messages
                        .Where(m => m.DeletedAt == null)
                        .OrderByDescending(m => m.CreatedAt)
                        .Take(messageLimit)
                })
                .Select(x => x.Session)
                .Include(s => s.Messages.Where(m => m.DeletedAt == null)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(messageLimit))
                .FirstOrDefaultAsync(ct);
        }

        // Load session without message limit (e.g., for adding new messages)
        return await query.FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<ChatSession>> GetByUserIdAsync(
        Guid userId, int page, int pageSize, CancellationToken ct = default)
    {
        return await context.Set<ChatSession>()
            .Where(s => s.UserId == userId && s.DeletedAt == null)
            .OrderByDescending(s => s.LastMessageAt ?? s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task AddMessagesAsync(IEnumerable<ChatMessage> messages, CancellationToken ct = default)
    {
        await context.Set<ChatMessage>().AddRangeAsync(messages, ct);
    }
}