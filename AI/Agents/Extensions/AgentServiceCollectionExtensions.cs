using AI.Agents.Enem;
using AI.Providers;
using AI.Providers.Memory;
using AI.Providers.Session;
using AI.Providers.Abstractions;
using Domain.Enums;
using Domain.Interfaces.Agents;
using Domain.ValueObjects;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AI.Agents.Extensions;

public static class AgentServiceCollectionExtensions
{
    /// <summary>
    /// Registra toda a infraestrutura de agentes:
    ///   - Pipeline MAF com logging, cache distribuído e function invocation
    ///   - IVectorStore (plugue sua implementação: Qdrant, AI Search, etc.)
    ///   - IPromptProvider keyed por tipo (File, Database, Remote)
    ///   - IAgentSession e IAgentMemory com IDistributedCache
    ///   - AgentOptions por agente via named options
    ///   - Todos os agentes concretos como internal (acessados via IAgent<,>)
    /// </summary>
    public static IServiceCollection AddAgents(
        this IServiceCollection services,
        Action<AgentInfraOptions>? configure = null)
    {
        var infraOptions = new AgentInfraOptions();
        configure?.Invoke(infraOptions);

        // ── IChatClient — pipeline MAF ───────────────────────────────────────
        if (infraOptions.ChatClientBuilder is not null)
        {
            services
                .AddChatClient(infraOptions.ChatClientBuilder)
                .UseLogging()
                .UseDistributedCache()
                .UseFunctionInvocation()
                .Build();
        }

        // ── IEmbeddingGenerator — para RAG ───────────────────────────────────
        if (infraOptions.EmbeddingGeneratorBuilder is not null)
        {
            services
                .AddEmbeddingGenerator(infraOptions.EmbeddingGeneratorBuilder)
                .UseLogging()
                .Build();
        }

        // ── IVectorStore — plugue sua implementação ──────────────────────────
        if (infraOptions.VectorStoreFactory is not null)
            services.AddSingleton<IVectorStore>(infraOptions.VectorStoreFactory);

        // ── IPromptProvider keyed por fonte ──────────────────────────────────
        services.AddKeyedSingleton<IPromptProvider, FilePromptProvider>(PromptProvider.File);

        // ── Session (scoped — uma por request/conversação) ───────────────────
        // UserId e SessionId podem ser sobrescritos na camada Presentation via
        // factory customizada que leia IHttpContextAccessor.
        services.AddScoped<IAgentSession>(sp =>
            new DistributedCacheAgentSession(
                sp.GetRequiredService<IDistributedCache>(),
                sp.GetRequiredService<ILogger<DistributedCacheAgentSession>>(),
                sessionId: Guid.NewGuid().ToString("N"),
                userId:    "anonymous",
                agentKey:  "default"));

        // ── Memory (singleton — persiste entre sessões) ──────────────────────
        services.AddSingleton<IAgentMemory, DistributedCacheAgentMemory>();

        // ── AgentOptions por agente (named options) ──────────────────────────
        services.AddOptions<AgentOptions>("enem-helper")
            .Configure(o =>
            {
                o.ModelId         = infraOptions.DefaultModelId;
                o.Temperature     = 0.7f;
                o.MaxTokens       = 1024;
                o.MaxHistory      = 10;
                o.EnableRag       = true;
                o.EnableMemory    = true;
                o.SystemPromptKey = "enem-helper";
            });

        services.AddOptions<AgentOptions>("enem-explainer")
            .Configure(o =>
            {
                o.ModelId         = infraOptions.DefaultModelId;
                o.Temperature     = 0.4f;
                o.MaxTokens       = 2048;
                o.EnableRag       = true;
                o.SystemPromptKey = "enem-explainer";
            });

        services.AddOptions<AgentOptions>("enem-question-generator")
            .Configure(o =>
            {
                o.ModelId         = infraOptions.DefaultModelId;
                o.Temperature     = 0.9f;
                o.MaxTokens       = 1500;
                o.EnableRag       = true;
                o.SystemPromptKey = "enem-question-generator";
            });

        services.AddOptions<AgentOptions>("enem-feedback")
            .Configure(o =>
            {
                o.ModelId         = infraOptions.DefaultModelId;
                o.Temperature     = 0.2f;
                o.MaxTokens       = 512;
                o.EnableMemory    = true;
                o.SystemPromptKey = "enem-feedback";
            });

        // ── Agentes concretos (transient — criados por request) ──────────────
        services.AddTransient<HelperEnemAgent>(sp => sp.ResolveAgent<HelperEnemAgent>("enem-helper"));
        services.AddTransient<ExplainerAgent> (sp => sp.ResolveAgent<ExplainerAgent> ("enem-explainer"));
        services.AddTransient<QuestionAgent>  (sp => sp.ResolveAgent<QuestionAgent>  ("enem-question-generator"));
        services.AddTransient<FeedbackAgent>  (sp => sp.ResolveAgent<FeedbackAgent>  ("enem-feedback"));

        // Exposição via interface para consumo externo
        services.AddTransient<IAgent<string, string>>(
            sp => sp.GetRequiredService<HelperEnemAgent>());

        services.AddTransient<IAgent<ExplainRequest, string>>(
            sp => sp.GetRequiredService<ExplainerAgent>());

        services.AddTransient<IAgent<QuestionRequest, EnemQuestion>>(
            sp => sp.GetRequiredService<QuestionAgent>());

        services.AddTransient<IAgent<FeedbackRequest, FeedbackResult>>(
            sp => sp.GetRequiredService<FeedbackAgent>());

        return services;
    }

    private static T ResolveAgent<T>(this IServiceProvider sp, string optionsName)
    {
        var namedOptions = sp.GetRequiredService<IOptionsSnapshot<AgentOptions>>()
                             .Get(optionsName);

        return ActivatorUtilities.CreateInstance<T>(sp, Options.Create(namedOptions));
    }
}

public sealed class AgentInfraOptions
{
    public string DefaultModelId { get; set; } = "gpt-4o-mini";

    /// <summary>Configure o IChatClient (OpenAI, AzureOpenAI, Ollama, etc.).</summary>
    public IChatClient? ChatClientBuilder { get; set; }

    /// <summary>Configure o IEmbeddingGenerator para RAG.</summary>
    public IEmbeddingGenerator<string, Embedding<float>>? EmbeddingGeneratorBuilder { get; set; }

    /// <summary>Fábrica do vector store. Ex: sp => new QdrantVectorStore(...).</summary>
    public Func<IServiceProvider, IVectorStore>? VectorStoreFactory { get; set; }
}
