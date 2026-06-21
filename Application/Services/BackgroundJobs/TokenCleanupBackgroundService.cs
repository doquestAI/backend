using Domain.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Services.BackgroundJobs;

internal class TokenCleanupBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<TokenCleanupBackgroundService> logger) : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Token cleanup background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var tokenRepository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();
                var dbCommit = scope.ServiceProvider.GetRequiredService<IDbCommit>();

                await tokenRepository.DeleteExpiredTokensAsync(stoppingToken);
                await dbCommit.Commit(stoppingToken);

                logger.LogInformation("Expired tokens cleanup completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during token cleanup background task");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        logger.LogInformation("Token cleanup background service stopped");
    }
}