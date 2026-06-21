using Domain.Common.Responses;
using MediatR;

namespace Application.UseCases.Commands.User.ForgotPassword;

public record Request(string Email) : IRequest<BaseResponse>;