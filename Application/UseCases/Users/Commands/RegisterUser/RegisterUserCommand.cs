using Application.Common;
using MediatR;

namespace Application.UseCases.Users.Commands.RegisterUser;

internal sealed record RegisterUserCommand(string FirebaseUid, string Email)
    : IRequest<Result<RegisterUserResponse>>;