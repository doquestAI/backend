using System.Diagnostics;
using AI.Providers.Abstractions;
using Domain.Enums;
using Domain.Interfaces.Agents;
using Domain.ValueObjects;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AI.Agents.Base;

/// <summary>
/// Classe base para TODOS os agentes do sistema.
///
/// Responsabilidades:
///   - Carregar system prompt via IPromptProvider
///   - Montar e gerenciar histórico de sessão (IAgentSession)
///   - Buscar contexto RAG antes de cada inferência (IVectorStore + IEmbeddingGenerator)
///   - Injetar memória de longo prazo (IAgentMemory)
///   - Executar o pipeline MAF (IChatClient com middlewares: retry, cache, logging, tracing)
///   - Métricas de latência e tokens
///   - Template method: subclasses implementam apenas RunCoreAsync
/// </summary>
public abstract class AgentBase<TIn, TOut> : IAgent<TIn, TOut>
{
    private   readonly IChatClient                                   _chatClient;
    private   readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGen;
    private   readonly IVectorStore                                   _vectorStore;
    private   readonly IPromptProvider                                _promptProvider;
    protected readonly IAgentSession                                  Session;
    protected readonly IAgentMemory                                   Memory;
    protected readonly ILogger                                        Logger;
    protected readonly AgentOptions                                   Options;

    private PromptTemplate? _systemPrompt;

    protected AgentBase(
        IChatClient                                      chatClient,
        IEmbeddingGenerator<string, Embedding<float>>   embeddingGen,
        IVectorStore                                     vectorStore,
        IPromptProvider                                  promptProvider,
        IAgentSession                                    session,
        IAgentMemory                                     memory,
        IOptions<AgentOptions>                           options,
        ILogger                                          logger)
    {
        _chatClient     = chatClient;
        _embeddingGen   = embeddingGen;
        _vectorStore    = vectorStore;
        _promptProvider = promptProvider;
        Session         = session;
        Memory          = memory;
        Options         = options.Value;
        Logger          = logger;
    }

    /// <summary>Chave usada para buscar o system prompt e identificar sessão.</summary>
    protected abstract string AgentKey { get; }

    /// <summary>Coleção do vector store usada por este agente para RAG. Retorne null para desabilitar RAG.</summary>
    protected virtual string? RagCollection => null;

    public async Task<TOut> RunAsync(TIn input, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            Logger.LogInformation("[{Agent}] Iniciando execução. SessionId={SessionId}",
                AgentKey, Session.SessionId);

            await EnsureSystemPromptAsync(cancellationToken);

            var processedInput = await PreProcessAsync(input, cancellationToken);
            var ragContext      = await RetrieveRagContextAsync(processedInput, cancellationToken);
            var memories        = await RetrieveMemoriesAsync(processedInput, cancellationToken);
            var messages        = await BuildMessagesAsync(processedInput, ragContext, memories, cancellationToken);
            var response        = await CompleteAsync(messages, cancellationToken);

            var inputText  = ExtractTextFromInput(processedInput);
            var outputText = response.Message.Text ?? string.Empty;
            await Session.AddMessageAsync(AgentMessage.User(inputText),       cancellationToken);
            await Session.AddMessageAsync(AgentMessage.Assistant(outputText), cancellationToken);

            await SaveToMemoryAsync(processedInput, outputText, cancellationToken);

            var result = await ParseResponseAsync(response, cancellationToken);

            sw.Stop();
            Logger.LogInformation(
                "[{Agent}] Execução concluída em {Elapsed}ms. Tokens={Tokens}",
                AgentKey, sw.ElapsedMilliseconds, response.Usage?.TotalTokenCount);

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "[{Agent}] Erro durante execução.", AgentKey);
            throw;
        }
    }

    protected virtual Task<TIn> PreProcessAsync(TIn input, CancellationToken ct)
        => Task.FromResult(input);

    protected virtual async Task<IList<ChatMessage>> BuildMessagesAsync(
        TIn input, string? ragContext, IReadOnlyList<string> memories, CancellationToken ct)
    {
        var messages = new List<ChatMessage>();

        if (_systemPrompt is not null)
        {
            var systemContent = BuildSystemContent(_systemPrompt, ragContext, memories);
            messages.Add(new ChatMessage(ChatRole.System, systemContent));
        }

        var history = await Session.GetHistoryAsync(Options.MaxHistory, ct);
        foreach (var msg in history)
        {
            var role = msg.Role switch
            {
                AgentRole.User      => ChatRole.User,
                AgentRole.Assistant => ChatRole.Assistant,
                _                   => ChatRole.System
            };
            messages.Add(new ChatMessage(role, msg.Content));
        }

        messages.Add(new ChatMessage(ChatRole.User, ExtractTextFromInput(input)));
        return messages;
    }

    protected virtual Task<ChatCompletion> CompleteAsync(IList<ChatMessage> messages, CancellationToken ct)
    {
        var chatOptions = new ChatOptions
        {
            ModelId         = Options.ModelId,
            Temperature     = Options.Temperature,
            MaxOutputTokens = Options.MaxTokens,
        };

        return _chatClient.CompleteAsync(messages, chatOptions, ct);
    }

    protected abstract Task<TOut> ParseResponseAsync(ChatCompletion response, CancellationToken ct);

    protected virtual string ExtractTextFromInput(TIn input) => input?.ToString() ?? string.Empty;

    protected virtual Task SaveToMemoryAsync(TIn input, string response, CancellationToken ct)
        => Task.CompletedTask;

    protected virtual IReadOnlyDictionary<string, string> GetPromptVariables()
        => new Dictionary<string, string>();

    private async Task<string?> RetrieveRagContextAsync(TIn input, CancellationToken ct)
    {
        if (!Options.EnableRag || RagCollection is null)
            return null;

        try
        {
            var queryText = ExtractTextFromInput(input);
            if (string.IsNullOrWhiteSpace(queryText)) return null;

            var embedding = await _embeddingGen.GenerateAsync([queryText], null, ct);
            var vector    = embedding.FirstOrDefault()?.Vector ?? ReadOnlyMemory<float>.Empty;

            if (vector.IsEmpty) return null;

            var results = await _vectorStore.SearchAsync(
                RagCollection, vector, Options.RagTopK, Options.RagThreshold, ct);

            if (!results.Any()) return null;

            var chunks = results
                .OrderByDescending(r => r.Score)
                .Select((r, i) => $"[{i + 1}] {r.Content}");

            return string.Join("\n\n", chunks);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "[{Agent}] RAG falhou, continuando sem contexto.", AgentKey);
            return null;
        }
    }

    private async Task<IReadOnlyList<string>> RetrieveMemoriesAsync(TIn input, CancellationToken ct)
    {
        if (!Options.EnableMemory)
            return Array.Empty<string>();

        try
        {
            var query = ExtractTextFromInput(input);
            return await Memory.SearchAsync(query, topK: 3, ct);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "[{Agent}] Memória falhou, continuando sem ela.", AgentKey);
            return Array.Empty<string>();
        }
    }

    private async Task EnsureSystemPromptAsync(CancellationToken ct)
    {
        if (_systemPrompt is not null) return;

        var key = Options.SystemPromptKey ?? AgentKey;
        if (await _promptProvider.ExistsAsync(key, ct))
            _systemPrompt = await _promptProvider.GetAsync(key, ct);
    }

    private string BuildSystemContent(
        PromptTemplate template,
        string? ragContext,
        IReadOnlyList<string> memories)
    {
        var vars = new Dictionary<string, string>(GetPromptVariables())
        {
            ["session_id"] = Session.SessionId,
            ["user_id"]    = Session.UserId,
            ["date_utc"]   = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
        };

        if (ragContext is not null)
            vars["rag_context"] = ragContext;

        if (memories.Count > 0)
            vars["memories"] = string.Join("\n", memories.Select((m, i) => $"- [{i + 1}] {m}"));

        return template.Render(vars);
    }
}
