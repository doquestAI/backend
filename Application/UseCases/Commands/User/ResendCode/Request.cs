using Domain.Common.Responses;
using MediatR;

namespace Application.UseCases.Commands.User.ResendCode;

internal record Request(string email, long token) : IRequest<BaseResponse>;