using AI.Agents.Enem;
using AI.Providers;
using AI.Providers.Abstractions;
using AI.Providers.Context;
using AI.Providers.Session;
using Domain.Agents.Enem;
using Domain.Enums;
using Domain.Interfaces.Agents;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AI.Agents.Extensions;

/// <summary>
/// Registro de DI dos agentes usando Microsoft Agent Framework (MAF) 1.x.
///
/// Cada agente concreto é registrado em duas camadas:
///   1. <see cref="AIAgent"/> (keyed por <see cref="AgentKeys"/>) — instância nativa do MAF
///      configurada com Instructions, ChatOptions, AIContextProviders e wrap de OpenTelemetryAgent.
///   2. Wrapper de domínio (<see cref="HelperEnemAgent"/>, etc.) — expõe
///      <see cref="IAgent{TIn,TOut}"/> tipado para a camada de aplicação.
/// </summary>
public static class AgentServiceCollectionExtensions
{
    public static IServiceCollection AddAgents(
        this IServiceCollection services,
        Action<AgentInfraOptions> configure)
    {
        var infra = new AgentInfraOptions();
        configure(infra);

        // ── Pipeline IChatClient (MEAI middleware) ──────────────────────────
        if (infra.ChatClientBuilder is not null)
        {
            services
                .AddChatClient(infra.ChatClientBuilder)
                .UseLogging()
                .UseDistributedCache()
                .UseFunctionInvocation();
        }

        // ── Pipeline IEmbeddingGenerator ────────────────────────────────────
        if (infra.EmbeddingGeneratorBuilder is not null)
        {
            services
                .AddEmbeddingGenerator(infra.EmbeddingGeneratorBuilder)
                .UseLogging();
        }

        // ── IVectorStore (Qdrant, Azure AI Search, etc.) ────────────────────
        if (infra.VectorStoreFactory is not null)
            services.AddSingleton(infra.VectorStoreFactory);

        // ── Prompt provider keyed (File por padrão) ─────────────────────────
        services.AddKeyedSingleton<IPromptProvider, FilePromptProvider>(PromptProvider.File);

        // ── Session cache compartilhado entre todos os agentes ──────────────
        services.AddSingleton<AgentSessionCache>();

        // ── Registra um AIAgent por nome usando AddKeyedSingleton ───────────
        RegisterEnemAgent(services, AgentKeys.Helper,
            ragCollection: "enem-knowledge", temperature: 0.7f, maxTokens: 1024, enableMemory: true);

        RegisterEnemAgent(services, AgentKeys.Explainer,
            ragCollection: "enem-knowledge", temperature: 0.4f, maxTokens: 2048, enableMemory: false);

        RegisterEnemAgent(services, AgentKeys.QuestionGenerator,
            ragCollection: "enem-knowledge", temperature: 0.9f, maxTokens: 1500, enableMemory: false);

        RegisterEnemAgent(services, AgentKeys.Feedback,
            ragCollection: null, temperature: 0.2f, maxTokens: 512, enableMemory: true);

        // ── Wrappers de domínio expostos via IAgent<TIn,TOut> ───────────────
        services.AddScoped<HelperEnemAgent>();
        services.AddScoped<ExplainerAgent>();
        services.AddScoped<QuestionAgent>();
        services.AddScoped<FeedbackAgent>();

        services.AddScoped<IAgent<string, string>>(sp => sp.GetRequiredService<HelperEnemAgent>());
        services.AddScoped<IStreamingAgent<string>>(sp => sp.GetRequiredService<HelperEnemAgent>());

        services.AddScoped<IAgent<ExplainRequest, string>>(sp => sp.GetRequiredService<ExplainerAgent>());
        services.AddScoped<IStreamingAgent<ExplainRequest>>(sp => sp.GetRequiredService<ExplainerAgent>());

        services.AddScoped<IAgent<QuestionRequest, EnemQuestion>>(sp => sp.GetRequiredService<QuestionAgent>());
        services.AddScoped<IAgent<FeedbackRequest, FeedbackResult>>(sp => sp.GetRequiredService<FeedbackAgent>());

        return services;
    }

    private static void RegisterEnemAgent(
        IServiceCollection services,
        string key,
        string? ragCollection,
        float temperature,
        int maxTokens,
        bool enableMemory)
    {
        services.AddKeyedSingleton<AIAgent>(key, (sp, _) =>
        {
            var chatClient = sp.GetRequiredService<IChatClient>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var promptProvider = sp.GetRequiredKeyedService<IPromptProvider>(PromptProvider.File);

            var instructions = LoadInstructions(promptProvider, key);
            var contextProviders = BuildContextProviders(sp, key, ragCollection, enableMemory);

            var options = new ChatClientAgentOptions
            {
                Name = key,
                ChatOptions = new ChatOptions
                {
                    Instructions = instructions,
                    Temperature = temperature,
                    MaxOutputTokens = maxTokens,
                },
                AIContextProviders = contextProviders,
            };

            AIAgent agent = new ChatClientAgent(chatClient, options, loggerFactory);

            // Decora com OpenTelemetryAgent: emite traces GenAI Semantic Conventions automaticamente.
            return new OpenTelemetryAgent(agent, sourceName: $"DoQuest.AI.{key}");
        });
    }

    private static List<AIContextProvider> BuildContextProviders(
        IServiceProvider sp,
        string agentKey,
        string? ragCollection,
        bool enableMemory)
    {
        var providers = new List<AIContextProvider>();

        if (ragCollection is not null)
        {
            providers.Add(new RagContextProvider(
                sp.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>(),
                sp.GetRequiredService<IVectorStore>(),
                ragCollection,
                sp.GetRequiredService<ILogger<RagContextProvider>>()));
        }

        if (enableMemory)
        {
            providers.Add(new DistributedCacheMemoryProvider(
                sp.GetRequiredService<IDistributedCache>(),
                memoryNamespace: agentKey,
                sp.GetRequiredService<ILogger<DistributedCacheMemoryProvider>>()));
        }

        return providers;
    }

    private static string LoadInstructions(IPromptProvider provider, string key)
    {
        if (!provider.ExistsAsync(key).GetAwaiter().GetResult())
            return string.Empty;

        var template = provider.GetAsync(key).GetAwaiter().GetResult();
        return template.Render(new Dictionary<string, string>
        {
            ["date_utc"] = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd"),
        });
    }
}

public sealed class AgentInfraOptions
{
    public IChatClient? ChatClientBuilder { get; set; }
    public IEmbeddingGenerator<string, Embedding<float>>? EmbeddingGeneratorBuilder { get; set; }
    public Func<IServiceProvider, IVectorStore>? VectorStoreFactory { get; set; }
}
