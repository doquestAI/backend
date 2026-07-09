using Application.Dtos;
using MediatR;

namespace Application.UseCases.Queries.Document.List;

internal record Request(Guid UserId, int PageNumber = 1, int PageSize = 10) : IRequest<Response>;
