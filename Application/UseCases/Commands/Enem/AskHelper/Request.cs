using MediatR;

namespace Application.UseCases.Commands.Enem.AskHelper;

public record Request(string Question) : IRequest<Response>;
