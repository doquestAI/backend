using MediatR;

namespace Application.UseCases.Queries.Chunk.Search;

internal record Request(
    string Query,
    int TopK = 10,
    float MinScore = 0.0f) : IRequest<Response>;
