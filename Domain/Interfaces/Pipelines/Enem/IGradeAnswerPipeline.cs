using Domain.Agents.Enem;

namespace Domain.Interfaces.Pipelines.Enem;

public interface IGradeAnswerPipeline : IPipeline<FeedbackRequest, FeedbackResult>;
