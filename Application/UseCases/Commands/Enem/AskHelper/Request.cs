using MediatR;

namespace Application.UseCases.Commands.Enem.AskHelper;

internal record Request(string Question) : IRequest<Response>;
