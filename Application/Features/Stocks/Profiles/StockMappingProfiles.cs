using Application.Common.Responses;
using Application.Features.Stocks.Commands.Create;
using Application.Features.Stocks.Commands.Delete;
using Application.Features.Stocks.Commands.Update;
using Application.Features.Stocks.Queries.GetById;
using Application.Features.Stocks.Queries.GetList;
using Application.Features.Stocks.Queries.GetListByDynamic;
using AutoMapper;
using Domain.Entities;
using Persistence.Paging;

namespace Application.Features.Stocks.Profiles;

public class StockMappingProfiles : Profile
{
    public StockMappingProfiles()
    {
        CreateMap<Stock, CreateStockCommand>().ReverseMap();
        CreateMap<Stock, CreatedStockResponse>().ReverseMap();

        CreateMap<Stock, UpdateStockCommand>().ReverseMap();
        CreateMap<Stock, UpdatedStockResponse>().ReverseMap();

        CreateMap<Stock, DeleteStockCommand>().ReverseMap();
        CreateMap<Stock, DeletedStockResponse>().ReverseMap();

        CreateMap<Stock, GetListStockListItemDto>()
            .ForMember(destinationMember: dest => dest.DrugName, memberOptions: opt => opt.MapFrom(src => src.Drug != null ? src.Drug.Name : string.Empty))
            .ForMember(destinationMember: dest => dest.WarehouseName, memberOptions: opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : string.Empty))
            .ReverseMap();

        CreateMap<Stock, GetListByDynamicStockListItemDto>()
            .ForMember(destinationMember: dest => dest.DrugName, memberOptions: opt => opt.MapFrom(src => src.Drug != null ? src.Drug.Name : string.Empty))
            .ForMember(destinationMember: dest => dest.WarehouseName, memberOptions: opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : string.Empty))
            .ReverseMap();

        CreateMap<Stock, GetByIdStockResponse>().ReverseMap();

        CreateMap<Paginate<Stock>, GetListResponse<GetListStockListItemDto>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.DataList ?? new List<Stock>()))
            .ForMember(dest => dest.DataList, opt => opt.Ignore()); ; ;

        CreateMap<Paginate<Stock>, GetListResponse<GetListByDynamicStockListItemDto>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.DataList ?? new List<Stock>()))
            .ForMember(dest => dest.DataList, opt => opt.Ignore());
    }
}
