using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Cache;

internal static class QueryCacheExtensions
{
    private const string NoCacheTag = "__NOCACHE__";

    public static IQueryable<T> NoCache<T>(this IQueryable<T> query) where T : class
    {
        return query.TagWith(NoCacheTag);
    }

    internal static bool IsNoCacheQuery(string commandText)
    {
        return commandText.Contains(NoCacheTag, StringComparison.Ordinal);
    }
}