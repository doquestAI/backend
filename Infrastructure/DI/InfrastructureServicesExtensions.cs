using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Domain.Configurations;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Interfaces.Vector;
using EFCoreSecondLevelCacheInterceptor;
using Infrastructure.Cache;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.Azure.BlobStorage;
using Infrastructure.Services.Azure.ServiceBus.Publishers;
using Infrastructure.Services.Azure.ServiceBus.Subscribers;
using Infrastructure.Services.Azure.ServiceBus.Subscribers.Factories;
using Infrastructure.Vector;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

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
            services.AddDistributedMemoryCache();
            services.AddScoped<IDbCommit, DbCommit>()
                .AddScoped<IMessagePublisher, ServiceBusMessagePublisher>()
                .AddScoped<ITemporaryStorageService, TemporaryStorageService>()
                .AddScoped<IEmailService, EmailService>()
                .AddScoped<ICloudStorageService, AzureBlobStorageService>()
                .AddScoped<IPictureRepository, PictureRepository>()
                .AddScoped<IDocumentRepository, DocumentRepository>()
                .AddScoped<IChunkRepository, ChunkRepository>()
                .AddScoped<IChatSessionRepository, ChatSessionRepository>()
                .AddScoped<IChatMessageRepository, ChatMessageRepository>()
                .AddScoped<IEmbeddingService, EmbeddingService>()
                .AddScoped<ChatHistoryService>()
                .AddScoped<IAccountSubscriptionRepository, AccountSubscriptionRepository>()
                .AddScoped<IIdentityProviderService, EntraIdentityProviderService>()
                .AddScoped<IStripeWebhookService, StripeWebhookService>()
                .AddScoped<IAgentSessionRepository, AgentSessionRepository>()
                .AddScoped<IVectorStore, PgVectorStore>();

            services.AddSingleton<GraphServiceClient>(provider =>
            {
                var settings = provider.GetRequiredService<IOptions<AzureAdSettings>>().Value;
                var credential = new ClientSecretCredential(settings.TenantId, settings.ClientId, settings.ClientSecret);
                return new GraphServiceClient(credential);
            });

            services.AddSingleton<ServiceBusClient>(provider =>
            {
                var settings = provider.GetRequiredService<IOptions<ServiceBusSettings>>().Value;
                return new ServiceBusClient(settings.ConnectionString);
            });

            services.AddSingleton<ServiceBusProcessorFactory>();
        }

        internal IServiceCollection AddServiceBusSubscribers()
        {
            services.AddHostedService<FileUploadSubscriber>();
            services.AddHostedService<EmbeddingCompletedSubscriber>();
            services.AddHostedService<EmbeddingDeletionCompletedSubscriber>();
            services.AddHostedService<EmailSenderSubscriber>();
            return services;
        }

        internal IServiceCollection AddAllBackgroundServices()
        {
            services.AddServiceBusSubscribers();
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
                        .UseNpgsql(
                            BuildConnectionStringFromSettings(settings),
                            o => o.UseVector())
                        .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());
                });
        }
    }
}