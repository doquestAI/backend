using MediatR;

namespace Application.UseCases.Commands.User.Register;

public record Request(string Email, string FirstName, string LastName,
    string PhoneNumber, long? Number, string? NeighBordHood,
    string? Road, string? Complement, string? CEP) : IRequest<Response>;