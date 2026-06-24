using AI.Providers.Session;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;

namespace AI.Agents.Base;

/// <summary>
/// Base genérica para wrappers de domínio sobre <see cref="AIAgent"/>.
///
/// Cuida do ciclo de vida da sessão (hidrata do cache → run → persiste no cache)
/// e expõe hooks tipados <see cref="FormatInput"/> e <see cref="ParseResponse"/>
/// para os agentes concretos.
///
/// O <see cref="AIAgent"/> injetado já vem totalmente configurado pelo DI com:
///   - Instructions (system prompt)
///   - AIContextProviders (RAG, memory)
///   - Tools (AIFunctionFactory)
///   - OpenTelemetryAgent (observabilidade GenAI semantic conventions)
/// </summary>
public abstract class AgentWrapperBase<TIn, TOut>(
    AIAgent inner,
    AgentSessionCache sessionCache)
{
    protected AIAgent Inner { get; } = inner;

    public async Task<TOut> RunAsync(
        TIn input,
        string? sessionKey = null,
        CancellationToken cancellationToken = default)
    {
        var prompt = FormatInput(input);
        var session = sessionKey is not null
            ? await sessionCache.LoadOrCreateAsync(Inner, sessionKey, cancellationToken)
            : null;

        var response = await Inner.RunAsync(prompt, session, cancellationToken: cancellationToken);

        if (session is not null && sessionKey is not null)
            await sessionCache.SaveAsync(Inner, session, sessionKey, cancellationToken);

        return ParseResponse(response);
    }

    public async IAsyncEnumerable<string> RunStreamingAsync(
        TIn input,
        string? sessionKey = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var prompt = FormatInput(input);
        var session = sessionKey is not null
            ? await sessionCache.LoadOrCreateAsync(Inner, sessionKey, cancellationToken)
            : null;

        await foreach (var update in Inner.RunStreamingAsync(
            prompt, session, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
                yield return update.Text;
        }

        if (session is not null && sessionKey is not null)
            await sessionCache.SaveAsync(Inner, session, sessionKey, cancellationToken);
    }

    /// <summary>Converte o input tipado em string para o LLM.</summary>
    protected abstract string FormatInput(TIn input);

    /// <summary>Converte a resposta do agente para o tipo de saída.</summary>
    protected abstract TOut ParseResponse(AgentResponse response);

    protected static string ResponseText(AgentResponse response)
        => string.Concat(response.Messages
            .Where(m => m.Role == ChatRole.Assistant)
            .SelectMany(m => m.Contents.OfType<TextContent>())
            .Select(c => c.Text));
}
