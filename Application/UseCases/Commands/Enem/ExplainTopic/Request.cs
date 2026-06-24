using MediatR;

namespace Application.UseCases.Commands.Enem.ExplainTopic;

public record Request(string Topic, string Area) : IRequest<Response>;
