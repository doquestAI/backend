using Domain.Common.Responses;
using MediatR;

namespace Application.UseCases.Commands.User.Update;

internal record UpdateUserRequest(
    Guid Id,
    string? FirstName,
    string? LastName,
    string? Road,
    string? NeighBordHood,
    long? Number,
    string CEP,
    string? Complement,
    bool? Active
) : IRequest<BaseResponse>;