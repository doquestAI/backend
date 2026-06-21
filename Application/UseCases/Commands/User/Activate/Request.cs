using Domain.Common.Responses;
using MediatR;

namespace Application.UseCases.Commands.User.Activate;

internal record Request(
    string email,
    string token,
    string password
) : IRequest<BaseResponse>;