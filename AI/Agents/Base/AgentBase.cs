using AI.Common;
using AI.Common.Config;
using AI.Observability;
using AI.Providers.Session;
using Domain.Agents;
using Domain.Agents.ValueObjects;
using Domain.Interfaces.Agents;
using Domain.Observability;
using Domain.Pipelines;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AI.Agents.Base;

/// <summary>
/// Classe base de todo Agent do sistema.
///
/// <para>
/// <b>Herda</b> de <see cref="DelegatingAIAgent"/> (que herda de <see cref="AIAgent"/>) —
/// pode ser usado em qualquer lugar que aceite <see cref="AIAgent"/>: tools, providers,
/// observabilidade, sessões nativas. Internamente delega para um <see cref="ChatClientAgent"/>
/// (sealed pelo MAF, daí o uso de delegação ao invés de inheritance direto).
/// </para>
///
/// <para>
/// <b>Implementa</b> <see cref="IAgent"/> do Domain para que a Pipeline (e a Application)
/// consigam invocá-lo de forma alto-nível sem conhecer o MAF.
/// </para>
///
/// <para>
/// <b>Observabilidade:</b> cada invocação emite um span OTel <c>Agent.Invoke/{Name}</c>
/// (filho do span do step que o chamou), logs estruturados via <see cref="ILogger"/> e
/// métricas de tokens/latência em <see cref="AgentTelemetry"/>.
/// </para>
/// </summary>
internal abstract class AgentBase : DelegatingAIAgent, IAgent
{
    public AgentId AgentId { get; }
    public AgentName AgentName { get; }
    public AgentRole Role { get; }
    public AgentStatus Status { get; protected set; } = AgentStatus.Idle;
    public AgentCapabilities Capabilities { get; }
    public AgentMetrics Metrics { get; } = new();

    private readonly AgentSessionStore _sessionStore;
    private readonly ILogger _logger;

    protected AgentBase(
        IChatClient chatClient,
        AgentConfig config,
        AgentSessionStore sessionStore,
        ILoggerFactory loggerFactory)
        : base(new ChatClientAgent(chatClient, BuildOptions(config), loggerFactory))
    {
        AgentId = AgentId.New();
        AgentName = new AgentName(config.Name);
        Role = config.Role;
        Capabilities = config.Capabilities ?? AgentCapabilities.Default();
        _sessionStore = sessionStore;
        _logger = loggerFactory.CreateLogger(GetType());
    }

    public async Task<AgentInvocationResult> InvokeAsync(
        AgentInvocationInput input, CancellationToken cancellationToken = default)
    {
        Status = AgentStatus.Running;
        var sw = Stopwatch.StartNew();

        var agentTags = new TagList
        {
            { TelemetryConstants.Tags.AgentName, AgentName.Value },
            { TelemetryConstants.Tags.AgentRole, Role.ToString() }
        };

        using var activity = AgentTelemetry.Source.StartActivity(
            $"Agent.Invoke/{AgentName.Value}", ActivityKind.Internal);

        activity?.SetTag(TelemetryConstants.Tags.AgentName, AgentName.Value);
        activity?.SetTag(TelemetryConstants.Tags.AgentId, AgentId.Value.ToString());
        activity?.SetTag(TelemetryConstants.Tags.AgentRole, Role.ToString());

        _logger.LogInformation(
            "Agent {AgentName} ({AgentRole}) invocado. SessionKey={SessionKey}",
            AgentName.Value, Role, input.SessionKey);

        AgentTelemetry.Invocations.Add(1, agentTags);

        try
        {
            AgentInvocationScope.UseRag = input.UseRag;

            AgentSession? session = input.SessionKey is null
                ? null
                : await _sessionStore.LoadOrCreateAsync(this, input.SessionKey, cancellationToken);

            var response = await RunAsync(input.Prompt, session, cancellationToken: cancellationToken);

            if (session is not null && input.SessionKey is not null)
                await _sessionStore.SaveAsync(this, session, input.SessionKey, AgentName.Value ?? string.Empty, cancellationToken);

            sw.Stop();
            var tokens = ExtractTokens(response);
            Metrics.RecordInvocation(tokens, sw.Elapsed);
            Status = AgentStatus.Done;

            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.SetTag(TelemetryConstants.Tags.LlmTokensInput, tokens.InputTokens);
            activity?.SetTag(TelemetryConstants.Tags.LlmTokensOutput, tokens.OutputTokens);

            AgentTelemetry.InputTokens.Add(tokens.InputTokens, agentTags);
            AgentTelemetry.OutputTokens.Add(tokens.OutputTokens, agentTags);
            AgentTelemetry.InvocationDuration.Record(sw.Elapsed.TotalMilliseconds, agentTags);

            _logger.LogInformation(
                "Agent {AgentName} concluído. InputTokens={InputTokens} OutputTokens={OutputTokens} Duration={DurationMs:F1}ms",
                AgentName.Value, tokens.InputTokens, tokens.OutputTokens, sw.Elapsed.TotalMilliseconds);

            return new AgentInvocationResult(ExtractText(response), tokens, sw.Elapsed);
        }
        catch (Exception ex)
        {
            sw.Stop();
            Status = AgentStatus.Done;

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddEvent(new ActivityEvent("exception", tags: new ActivityTagsCollection
            {
                ["exception.type"] = ex.GetType().FullName,
                ["exception.message"] = ex.Message,
                ["exception.stacktrace"] = ex.ToString()
            }));

            AgentTelemetry.Failures.Add(1, agentTags);
            AgentTelemetry.InvocationDuration.Record(sw.Elapsed.TotalMilliseconds, agentTags);

            _logger.LogError(ex,
                "Agent {AgentName} falhou após {DurationMs:F1}ms. SessionKey={SessionKey}",
                AgentName.Value, sw.Elapsed.TotalMilliseconds, input.SessionKey);

            throw;
        }
    }

    public async IAsyncEnumerable<string> InvokeStreamingAsync(
        AgentInvocationInput input,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Status = AgentStatus.Running;
        var sw = Stopwatch.StartNew();

        var agentTags = new TagList
        {
            { TelemetryConstants.Tags.AgentName, AgentName.Value },
            { TelemetryConstants.Tags.AgentRole, Role.ToString() }
        };

        using var activity = AgentTelemetry.Source.StartActivity(
            $"Agent.InvokeStreaming/{AgentName.Value}", ActivityKind.Internal);

        activity?.SetTag(TelemetryConstants.Tags.AgentName, AgentName.Value);
        activity?.SetTag(TelemetryConstants.Tags.AgentId, AgentId.Value.ToString());

        _logger.LogInformation(
            "Agent {AgentName} streaming iniciado. SessionKey={SessionKey}",
            AgentName.Value, input.SessionKey);

        AgentTelemetry.Invocations.Add(1, agentTags);

        AgentSession? session = input.SessionKey is null
            ? null
            : await _sessionStore.LoadOrCreateAsync(this, input.SessionKey, cancellationToken);

        await foreach (var update in RunStreamingAsync(
            input.Prompt, session, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
                yield return update.Text;
        }

        if (session is not null && input.SessionKey is not null)
            await _sessionStore.SaveAsync(this, session, input.SessionKey, AgentName.Value ?? string.Empty, cancellationToken);

        sw.Stop();
        Metrics.RecordInvocation(TokenUsage.Empty, sw.Elapsed);
        Status = AgentStatus.Done;

        activity?.SetStatus(ActivityStatusCode.Ok);
        AgentTelemetry.InvocationDuration.Record(sw.Elapsed.TotalMilliseconds, agentTags);

        _logger.LogInformation(
            "Agent {AgentName} streaming concluído. Duration={DurationMs:F1}ms",
            AgentName.Value, sw.Elapsed.TotalMilliseconds);
    }

    private static ChatClientAgentOptions BuildOptions(AgentConfig config) => new()
    {
        Name = config.Name,
        Description = config.Description,
        ChatOptions = new ChatOptions
        {
            Instructions = config.SystemPrompt,
            Temperature = config.Temperature,
            MaxOutputTokens = config.MaxOutputTokens,
        },
        AIContextProviders = config.ContextProviders,
    };

    private static string ExtractText(AgentResponse response) =>
        string.Concat(response.Messages
            .Where(m => m.Role == ChatRole.Assistant)
            .SelectMany(m => m.Contents.OfType<TextContent>())
            .Select(c => c.Text));

    private static TokenUsage ExtractTokens(AgentResponse response)
    {
        var usage = response.Usage;
        if (usage is null)
            return TokenUsage.Empty;
        return new TokenUsage(
            usage.InputTokenCount ?? 0,
            usage.OutputTokenCount ?? 0);
    }
}
