using Application.Common.Responses;
using Application.Features.Orders.Commands.Create;
using Application.Features.Orders.Commands.Delete;
using Application.Features.Orders.Commands.Update;
using Application.Features.Orders.Queries.GetById;
using Application.Features.Orders.Queries.GetList;
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

        CreateMap<Order, GetListOrderListItemDto>().ReverseMap();
        CreateMap<Order, GetByIdOrderResponse>().ReverseMap();

        CreateMap<Paginate<Order>, GetListResponse<GetListOrderListItemDto>>().ReverseMap();
    }
}
