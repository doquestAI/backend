using DoQuest.Application.Common;
using MediatR;

namespace DoQuest.Application.UseCases.Users.Commands.RegisterUser;

public sealed record RegisterUserCommand(string FirebaseUid, string Email)
    : IRequest<Result<RegisterUserResponse>>;
