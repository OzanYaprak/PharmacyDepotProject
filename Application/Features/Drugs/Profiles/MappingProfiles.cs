using Application.Features.Drugs.Commands.Create;
using Application.Features.Drugs.Commands.Delete;
using Application.Features.Drugs.Commands.Update;
using Application.Features.Drugs.Queries.GetById;
using Application.Features.Drugs.Queries.GetList;
using Application.Features.Drugs.Responses;
using AutoMapper;
using Domain.Entities;
using Persistence.Paging;

namespace Application.Features.Drugs.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Drug, CreateDrugCommand>().ReverseMap();
        CreateMap<Drug, CreatedDrugResponse>().ReverseMap();

        CreateMap<Drug, UpdateDrugCommand>().ReverseMap();
        CreateMap<Drug, UpdateDrugResponse>().ReverseMap();

        CreateMap<Drug, DeleteDrugCommand>().ReverseMap();
        CreateMap<Drug, DeleteDrugResponse>().ReverseMap();

        CreateMap<Drug, GetListDrugListItemDTO>().ReverseMap();
        CreateMap<Drug, GetByIdDrugResponse>().ReverseMap();

        CreateMap<Paginate<Drug>, GetListResponse<GetListDrugListItemDTO>>().ReverseMap();
    }
}