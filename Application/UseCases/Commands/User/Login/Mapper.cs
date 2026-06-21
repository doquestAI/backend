using AutoMapper;
using Domain.ValueObjects;

namespace Application.UseCases.Commands.User.Login;

public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<Request, Domain.Entities.Core.User>()
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => new Password(src.password, true)))
            .ConstructUsing(request => new Domain.Entities.Core.User(
                new Domain.ValueObjects.Email(request.email),
                new Password(request.password, true)
            ));

        CreateMap<Domain.Entities.Core.User, ResponseUser>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Email", opt => opt.MapFrom(src => src.Email.Address))
            .ForCtorParam("FullName", opt => opt.MapFrom(src => $"{src.FullName.FirstName} {src.FullName.LastName}"))
            .ForCtorParam("Roles", opt => opt.MapFrom(src =>
                src.UserRoles.Select(ur => ur.Role.Name.Name)));
    }
}