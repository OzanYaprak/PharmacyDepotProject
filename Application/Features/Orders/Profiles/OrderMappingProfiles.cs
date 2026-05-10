using Application.Common.Responses;
using Application.Features.Orders.Commands.Create;
using Application.Features.Orders.Commands.Delete;
using Application.Features.Orders.Commands.Update;
using Application.Features.Orders.Queries.GetById;
using Application.Features.Orders.Queries.GetList;
using Application.Features.Orders.Queries.GetListByDynamic;
using AutoMapper;
using Domain.Entities;
using Persistence.Paging;

namespace Application.Features.Orders.Profiles;

public class OrderMappingProfiles : Profile
{
    public OrderMappingProfiles()
    {
        CreateMap<Order, CreateOrderCommand>().ReverseMap();
        CreateMap<Order, CreatedOrderResponse>().ReverseMap();

        CreateMap<Order, UpdateOrderCommand>().ReverseMap();
        CreateMap<Order, UpdatedOrderResponse>().ReverseMap();

        CreateMap<Order, DeleteOrderCommand>().ReverseMap();
        CreateMap<Order, DeletedOrderResponse>().ReverseMap();

        CreateMap<Order, GetListOrderListItemDto>()
            .ForMember(destinationMember: dest => dest.SupplierName, memberOptions: opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : string.Empty))
            .ReverseMap();

        CreateMap<Order, GetListByDynamicOrderListItemDto>()
            .ForMember(destinationMember: dest => dest.SupplierName, memberOptions: opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : string.Empty))
            .ReverseMap();

        CreateMap<Order, GetByIdOrderResponse>().ReverseMap();

        CreateMap<Paginate<Order>, GetListResponse<GetListOrderListItemDto>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.DataList ?? new List<Order>()))
            .ForMember(dest => dest.DataList, opt => opt.Ignore());

        CreateMap<Paginate<Order>, GetListResponse<GetListByDynamicOrderListItemDto>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.DataList ?? new List<Order>()))
            .ForMember(dest => dest.DataList, opt => opt.Ignore());
    }
}