using Application.Common.Responses;
using Application.Features.Sales.Commands.Create;
using Application.Features.Sales.Commands.Delete;
using Application.Features.Sales.Commands.Update;
using Application.Features.Sales.Queries.GetById;
using Application.Features.Sales.Queries.GetList;
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

        CreateMap<Sale, GetListSaleListItemDto>().ReverseMap();
        CreateMap<Sale, GetByIdSaleResponse>().ReverseMap();

        CreateMap<Paginate<Sale>, GetListResponse<GetListSaleListItemDto>>().ReverseMap();
    }
}
