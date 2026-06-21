using Application.Dtos;

namespace Application.UseCases.Queries.User.GetAll;

internal record Request : PaginatedRequest<Response>
{
    public Request() : base() { }
    public Request(int pageNumber, int pageSize) : base(pageNumber, pageSize) { }
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
}