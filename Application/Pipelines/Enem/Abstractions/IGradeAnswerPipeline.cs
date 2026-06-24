using Application.Pipelines.Abstractions;
using Domain.Agents.Enem;

namespace Application.Pipelines.Enem.Abstractions;

public interface IGradeAnswerPipeline : IPipeline<FeedbackRequest, FeedbackResult>;
