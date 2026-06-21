using Domain.Common.Responses;
using MediatR;

namespace Application.UseCases.Commands.User.ForgotPassword.Activate;

internal record Request(string? token, string newPassword, string email) : IRequest<BaseResponse>;