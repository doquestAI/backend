namespace Application.UseCases.Commands.User.Login;

internal record ResponseUser(
    Guid Id,
    string Email,
    string FullName,
    IEnumerable<string> Roles
);