using Domain.Common.Responses;
using MediatR;

namespace Application.UseCases.Commands.User.Delete;

internal record DeleteUserRequest(
    Guid Id
) : IRequest<BaseResponse>;