using Application.Common.Responses;
using Application.Features.Sales.Commands.Create;
using Application.Features.Sales.Commands.Delete;
using Application.Features.Sales.Commands.Update;
using Application.Features.Sales.Queries.GetById;
using Application.Features.Sales.Queries.GetList;
using Application.Features.Sales.Queries.GetListByDynamic;
using AutoMapper;
using Domain.Entities;
using Persistence.Paging;

namespace Application.Features.Sales.Profiles;

public class SaleMappingProfiles : Profile
{
    public SaleMappingProfiles()
    {
        CreateMap<Sale, CreateSaleCommand>().ReverseMap();
        CreateMap<Sale, CreatedSaleResponse>().ReverseMap();

        CreateMap<Sale, UpdateSaleCommand>().ReverseMap();
        CreateMap<Sale, UpdatedSaleResponse>().ReverseMap();

        CreateMap<Sale, DeleteSaleCommand>().ReverseMap();
        CreateMap<Sale, DeletedSaleResponse>().ReverseMap();

        CreateMap<Sale, GetListSaleListItemDto>()
            .ForMember(destinationMember: dest => dest.CustomerName, memberOptions: opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : string.Empty))
            .ReverseMap();

        CreateMap<Sale, GetListByDynamicSaleListItemDto>()
            .ForMember(destinationMember: dest => dest.CustomerName, memberOptions: opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : string.Empty))
            .ReverseMap();

        CreateMap<Sale, GetByIdSaleResponse>().ReverseMap();

        CreateMap<Paginate<Sale>, GetListResponse<GetListSaleListItemDto>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.DataList ?? new List<Sale>()))
            .ForMember(dest => dest.DataList, opt => opt.Ignore()); ;

        CreateMap<Paginate<Sale>, GetListResponse<GetListByDynamicSaleListItemDto>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.DataList ?? new List<Sale>()))
            .ForMember(dest => dest.DataList, opt => opt.Ignore());
    }
}
