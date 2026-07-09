using MediatR;

namespace Application.UseCases.Queries.Document.GetById;

internal record Request(Guid DocumentId) : IRequest<Response>;
