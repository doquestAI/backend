namespace Domain.Observability;

/// <summary>
/// Nomes canônicos de ActivitySources, Meters e tags semânticas usados em toda a aplicação.
/// Seguem as OTel Semantic Conventions onde aplicável.
/// </summary>
public static class TelemetryConstants
{
    public static class Sources
    {
        public const string Pipeline = "Doquest.Pipeline";
        public const string Agent   = "Doquest.Agent";
    }

    public static class Meters
    {
        public const string Pipeline = "Doquest.Pipeline";
        public const string Agent    = "Doquest.Agent";
    }

    /// <summary>Tags usadas em spans e métricas. Prefixos seguem as OTel Semantic Conventions.</summary>
    public static class Tags
    {
        // Pipeline
        public const string PipelineName      = "pipeline.name";
        public const string PipelineId        = "pipeline.id";
        public const string PipelineStatus    = "pipeline.status";
        public const string PipelineStepCount = "pipeline.step_count";

        // Step
        public const string StepName  = "pipeline.step.name";
        public const string StepIndex = "pipeline.step.index";

        // Agent (semântica gen AI — OTel GenAI Semantic Conventions)
        public const string AgentName = "gen_ai.agent.name";
        public const string AgentId   = "gen_ai.agent.id";
        public const string AgentRole = "gen_ai.agent.role";

        // LLM tokens
        public const string LlmTokensInput  = "gen_ai.usage.input_tokens";
        public const string LlmTokensOutput = "gen_ai.usage.output_tokens";

        // Geral
        public const string UserId    = "enduser.id";
        public const string Outcome   = "outcome";   // "success" | "failure"
        public const string ErrorType = "error.type";
    }
}
