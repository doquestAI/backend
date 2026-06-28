using MediatR;

namespace Application.UseCases.Commands.Enem.ExplainTopic;

internal record Request(string Topic, string Area) : IRequest<Response>;
