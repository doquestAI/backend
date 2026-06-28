using AI.Agents.Enem;
using AI.Capabilities.Extensions;
using AI.Common.Config;
using AI.Pipelines.Enem;
using AI.Providers;
using AI.Providers.Context;
using AI.Providers.Session;
using Domain.Agents;
using Domain.Enums;
using Domain.Interfaces.Pipelines.Enem;
using Domain.Interfaces.Prompts;
using Domain.Interfaces.Vector;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AI.Agents.Extensions;

/// <summary>
/// Registro de DI dos Agents e Pipelines.
///
/// Agora cada agente concreto é UMA LINHA — herda <c>AgentBase : ChatClientAgent</c>
/// e recebe um <see cref="AgentConfig"/> keyed via DI. Não há mais wrappers ou
/// composição manual de inner+session+resolver.
/// </summary>
internal static class AgentServiceCollectionExtensions
{
    internal static IServiceCollection AddAgents(
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

        // ── Prompt provider keyed (File por padrão) ─────────────────────────
        services.AddKeyedSingleton<IPromptProvider, FilePromptProvider>(PromptProvider.File);

        // ── Session store (DB-backed via IAgentSessionRepository) ───────────
        services.AddScoped<AgentSessionStore>();
        services.AddAgentCapabilities();

        // ── Configuração keyed de cada Agent ENEM ───────────────────────────
        RegisterAgentConfig(services, AgentKeys.Helper, AgentRole.Helper,
            description: "Responde dúvidas gerais sobre o ENEM",
            ragCollection: "enem-knowledge", temperature: 0.7f, maxTokens: 1024, enableMemory: true);

        RegisterAgentConfig(services, AgentKeys.Explainer, AgentRole.Specialist,
            description: "Explica teoria de tópicos do ENEM em detalhes",
            ragCollection: "enem-knowledge", temperature: 0.4f, maxTokens: 2048, enableMemory: false);

        RegisterAgentConfig(services, AgentKeys.QuestionGenerator, AgentRole.Generator,
            description: "Gera questões estilo ENEM com gabarito",
            ragCollection: "enem-knowledge", temperature: 0.9f, maxTokens: 1500, enableMemory: false);

        RegisterAgentConfig(services, AgentKeys.Feedback, AgentRole.Evaluator,
            description: "Corrige e avalia respostas do candidato",
            ragCollection: null, temperature: 0.2f, maxTokens: 512, enableMemory: true);

        // ── Agents concretos (cada um é uma linha herdando AgentBase) ───────
        services.AddScoped<HelperEnemAgent>();
        services.AddScoped<ExplainerAgent>();
        services.AddScoped<QuestionAgent>();
        services.AddScoped<FeedbackAgent>();

        // ── Pipelines concretas (Domain interfaces) ─────────────────────────
        services.AddScoped<IGenerateQuestionPipeline, GenerateQuestionPipeline>();
        services.AddScoped<IExplainTopicPipeline, ExplainTopicPipeline>();
        services.AddScoped<IGradeAnswerPipeline, GradeAnswerPipeline>();
        services.AddScoped<IAskHelperPipeline, AskHelperPipeline>();

        return services;
    }

    private static void RegisterAgentConfig(
        IServiceCollection services,
        string key,
        AgentRole role,
        string description,
        string? ragCollection,
        float temperature,
        int maxTokens,
        bool enableMemory)
    {
        services.AddKeyedSingleton(key, (sp, _) =>
        {
            var promptProvider = sp.GetRequiredKeyedService<IPromptProvider>(PromptProvider.File);
            var instructions = LoadInstructions(promptProvider, key);
            var contextProviders = BuildContextProviders(sp, key, ragCollection, enableMemory);

            return new AgentConfig
            {
                Name = key,
                Description = description,
                Role = role,
                SystemPrompt = instructions,
                Temperature = temperature,
                MaxOutputTokens = maxTokens,
                ContextProviders = contextProviders,
                Capabilities = new AgentCapabilities
                {
                    SupportsStreaming = true,
                    SupportsFunctionCalling = true,
                    SupportsRag = ragCollection is not null,
                    SupportsLongTermMemory = enableMemory,
                    SupportsMcp = false,
                },
            };
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

internal sealed class AgentInfraOptions
{
    public IChatClient? ChatClientBuilder { get; set; }
    public IEmbeddingGenerator<string, Embedding<float>>? EmbeddingGeneratorBuilder { get; set; }
}
