using Application.Features.Drugs.Commands.Create;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.Drugs.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Drug, CreateDrugCommand>().ReverseMap();
        CreateMap<Drug, CreatedDrugResponse>().ReverseMap();
    }
}
