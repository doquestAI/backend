using MediatR;

namespace Application.UseCases.Commands.User.Login;

public record Request(
    string email,
    string password,
    string? DeviceInfo = null,
    string? IpAddress = null) : IRequest<Response>;