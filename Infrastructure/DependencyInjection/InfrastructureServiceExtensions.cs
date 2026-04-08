using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Interfaces.Services.AI;
using Domain.Interfaces.Services.AI.Embeddings;
using Domain.Interfaces.Services.Chunkers;
using Domain.Interfaces.Services.Cloud.Auth;
using Domain.Interfaces.Services.Cloud.Storage;
using Domain.Interfaces.Services.Email;
using Domain.Interfaces.Services.Payment;
using EFCoreSecondLevelCacheInterceptor;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Infrastructure.AI;
using Infrastructure.Auth;
using Infrastructure.Cache;
using Infrastructure.Configurations;
using Infrastructure.Options;
using Infrastructure.Payments;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.GoogleCloud.PubSub.Publishers;
using Infrastructure.Services.GoogleCloud.PubSub.Subscribers;
using Infrastructure.Services.GoogleCloud.PubSub.Subscribers.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.DependencyInjection;

internal static class InfrastructureServiceExtensions
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
            services.AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IMessagePublisher, PubSubMessagePublisher>()
                .AddScoped<ITemporaryStorageService, TemporaryStorageService>()
                .AddScoped<IEmailService, EmailService>()
                .AddScoped<ICloudStorageService, GoogleCloudStorageService>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IVestibularRepository, VestibularRepository>()
                .AddScoped<IPlanRepository, PlanRepository>()
                .AddScoped<IDocumentRepository, DocumentRepository>()
                .AddScoped<IChatSessionRepository, ChatSessionRepository>()
                .AddScoped<IEmbeddingService, OllamaEmbeddingService>()
                .AddScoped<IChatAgentService, OllamaChatAgentService>()
                .AddScoped<IAuthService, FirebaseAuthService>()
                .AddScoped<IGatewayPaymentService, StripeService>()
                .AddScoped<IDocumentChunkerService, FixedWindowDocumentChunkerService>();
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

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<CoreDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                //npgsqlOptions.UseVector();
                npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
            })
            .UseSnakeCaseNamingConvention());


        InitializeFirebase(configuration);
        return services;
    }

    private static void InitializeFirebase(IConfiguration configuration)
    {
        if (FirebaseApp.DefaultInstance is not null)
            return;

        var serviceAccountJson = configuration[$"{FirebaseOptions.SectionName}:ServiceAccountJson"];

        if (!string.IsNullOrWhiteSpace(serviceAccountJson))
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(serviceAccountJson)
            });
        }
        else
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.GetApplicationDefault()
            });
        }
    }
}