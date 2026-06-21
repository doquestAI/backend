using Domain.Configurations;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Cache;

internal static class CacheServiceExtensions
{
    internal static IServiceCollection AddEFCoreCacheWithProvider(
        this IServiceCollection services,
        CacheSettings settings)
    {
        return settings.Provider switch
        {
            CacheProvider.Memory => services.AddEFSecondLevelCacheForMemory(settings),
            CacheProvider.Redis => throw new NotImplementedException(
                "Redis cache provider not implemented yet. Set CacheSettings.Provider = 0 (Memory)"),
            _ => throw new ArgumentOutOfRangeException(nameof(settings.Provider), "Unknown cache provider")
        };
    }

    private static IServiceCollection AddEFSecondLevelCacheForMemory(
        this IServiceCollection services,
        CacheSettings settings)
    {
        services.AddEFSecondLevelCache(options =>
            options.UseMemoryCacheProvider()
                .ConfigureLogging(false)
                .UseCacheKeyPrefix("EF_")
                .CacheAllQueries(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(settings.DefaultExpirationMinutes))
                .SkipCachingCommands(commandText => QueryCacheExtensions.IsNoCacheQuery(commandText))
        );

        return services;
    }
}