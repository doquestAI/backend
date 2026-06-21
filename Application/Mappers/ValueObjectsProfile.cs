using AutoMapper;
using Domain.ValueObjects;

namespace Application.Mappers;

public class ValueObjectsProfile : Profile
{
    public ValueObjectsProfile()
    {
        CreateMap<Email, string>()
            .ConvertUsing(src => src.Address ?? string.Empty);

        CreateMap<FullName, string>()
            .ConvertUsing(src => $"{src.FirstName} {src.LastName}");

        CreateMap<Cpf, string>()
            .ConvertUsing(src => src.Numero ?? string.Empty);

        CreateMap<CNPJ, string>()
            .ConvertUsing(src => src.Number ?? string.Empty);

        CreateMap<UniqueName, string>()
            .ConvertUsing(src => src.Name ?? string.Empty);

        CreateMap<Description, string>()
            .ConvertUsing(src => src.Text ?? string.Empty);

        CreateMap<Url, string>()
            .ConvertUsing(src => src.Endereco ?? string.Empty);
    }
}