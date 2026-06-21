using MediatR;

namespace Application.UseCases.Commands.Document.Delete;

public record Request(Guid DocumentId) : IRequest<Response>;