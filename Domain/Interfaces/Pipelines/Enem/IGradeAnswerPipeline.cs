using Domain.Agents.Enem;

namespace Domain.Interfaces.Pipelines.Enem;

internal interface IGradeAnswerPipeline : IPipeline<FeedbackRequest, FeedbackResult>;
