namespace Domain.Interfaces.Context;

public interface IUserContext
{
    Guid UserId { get; }
    string Role { get; }
    string Email { get; }
}