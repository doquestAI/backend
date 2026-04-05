using DoQuest.Application.Abstractions;
using DoQuest.Domain.Repositories;
using DoQuest.Infrastructure.AI;
using DoQuest.Infrastructure.Auth;
using DoQuest.Infrastructure.Options;
using DoQuest.Infrastructure.Payments;
using DoQuest.Infrastructure.Persistence;
using DoQuest.Infrastructure.Persistence.Repositories;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DoQuest.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Options
        services.Configure<OllamaOptions>(configuration.GetSection(OllamaOptions.SectionName));
        services.Configure<FirebaseOptions>(configuration.GetSection(FirebaseOptions.SectionName));
        services.Configure<StripeOptions>(configuration.GetSection(StripeOptions.SectionName));

        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<DoQuestDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.UseVector();
                npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
            })
            .UseSnakeCaseNamingConvention());

        // Repositories (Scoped — one per HTTP request)
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IVestibularRepository, VestibularRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // AI Services
        services.AddScoped<IEmbeddingService, OllamaEmbeddingService>();
        services.AddScoped<IChatAgentService, OllamaChatAgentService>();
        services.AddSingleton<IDocumentChunker, FixedWindowDocumentChunker>();

        // Firebase (singleton — initialized once)
        InitializeFirebase(configuration);
        services.AddScoped<IFirebaseAuthService, FirebaseAuthService>();

        // Stripe
        services.AddScoped<IStripeService, StripeService>();

        return services;
    }

    private static void InitializeFirebase(IConfiguration configuration)
    {
        if (FirebaseApp.DefaultInstance is not null) return;

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
            // Fallback to Application Default Credentials (useful in CI/Cloud)
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.GetApplicationDefault()
            });
        }
    }
}
