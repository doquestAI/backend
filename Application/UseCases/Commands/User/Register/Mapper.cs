using AutoMapper;
using Domain.ValueObjects;

namespace Application.UseCases.Commands.User.Register;

public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<string, Password>()
            .ConstructUsing(password => new Password(password, false));

        CreateMap<Request, Domain.Entities.Core.User>()
            .ConstructUsing((request, context) =>
            {
                return new Domain.Entities.Core.User(
                    new FullName(request.FirstName, request.LastName),
                    new Domain.ValueObjects.Email(request.Email),
                    new Address(request.Number, request.NeighBordHood, request.Road, request.Complement, request.CEP)
                );
            });

        CreateMap<Domain.Entities.Core.User, Response>()
            .ConstructUsing(user => new Response(
                200,
                "User created successfully. An email has been sent to activate the account",
                user.Notifications.ToList(),
                null
            ));
    }
}