using MediatR;

namespace Application.UseCases.Queries.User.GetOne;

internal record Request(Guid Id) : IRequest<Response>;