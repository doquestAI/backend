using Domain.Configurations;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using EFCoreSecondLevelCacheInterceptor;
using Infrastructure.Cache;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.GoogleCloud.PubSub.Publishers;
using Infrastructure.Services.GoogleCloud.PubSub.Subscribers;
using Infrastructure.Services.GoogleCloud.PubSub.Subscribers.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.DI;

internal static class InfrastructureServicesExtensions
{
    private static string BuildConnectionStringFromSettings(DatabaseSettings settings)
    {
        var builder = new Npgsql.NpgsqlConnectionStringBuilder
        {
            Host = settings.Host,
            Port = settings.Port,
            Database = settings.Database,
            Username = settings.Username,
            Password = settings.Password
        };
        return builder.ConnectionString;
    }

    extension(IServiceCollection services)
    {
        internal void ConfigureInfrastructureServices()
        {
            services.AddMemoryCache();
            services.AddScoped<IDbCommit, DbCommit>()
                .AddScoped<IMessagePublisher, PubSubMessagePublisher>()
                .AddScoped<ITemporaryStorageService, TemporaryStorageService>()
                .AddScoped<IEmailService, EmailService>()
                .AddScoped<ITokenService, TokenService>()
                .AddScoped<ICloudStorageService, GoogleCloudStorageService>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
                .AddScoped<IPictureRepository, PictureRepository>()
                .AddScoped<IRoleRepository, RoleRepository>()
                .AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddSingleton<SubscriberFactory>();
        }

        internal IServiceCollection AddPubSubSubscribers()
        {
            services.AddHostedService<FileUploadSubscriber>();
            services.AddHostedService<EmbeddingCompletedSubscriber>();
            services.AddHostedService<EmbeddingDeletionCompletedSubscriber>();
            services.AddHostedService<EmailSenderSubscriber>();
            return services;
        }

        internal IServiceCollection AddAllBackgroundServices()
        {
            services.AddPubSubSubscribers();
            return services;
        }
        internal void AddCacheServices(CacheSettings cacheSettings)
        {
            services.AddEFCoreCacheWithProvider(cacheSettings);
        }

        internal void AddDataContexts()
        {
            services
                .AddDbContext<CoreDbContext>((serviceProvider, optionsBuilder) =>
                {
                    var settings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                    optionsBuilder
                        .UseNpgsql(BuildConnectionStringFromSettings(settings))
                        .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());
                });
        }
    }
}