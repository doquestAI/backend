using MediatR;

namespace Application.UseCases.Commands.Document.Delete;

internal record Request(Guid DocumentId) : IRequest<Response>;