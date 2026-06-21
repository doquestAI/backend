
using MediatR;

namespace Application.Dtos;

public abstract record PaginatedRequest<TResponse> : IRequest<TResponse>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    protected PaginatedRequest() { }

    protected PaginatedRequest(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize < 1 ? 10 : pageSize > 100 ? 100 : pageSize;
    }
}