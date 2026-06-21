using Domain.Common.Responses;
using MediatR;

namespace Application.UseCases.Commands.User.Logout;

public record Request(string AccessToken, string? RefreshToken = null) : IRequest<BaseResponse>;