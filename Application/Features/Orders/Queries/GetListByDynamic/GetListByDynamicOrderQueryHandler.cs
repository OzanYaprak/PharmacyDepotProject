using Application.Common.Responses;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Paging;
using Persistence.Repositories.Order;

namespace Application.Features.Orders.Queries.GetListByDynamic;

public class GetListByDynamicOrderQueryHandler : IRequestHandler<GetListByDynamicOrderQuery, GetListResponse<GetListByDynamicOrderListItemDto>>
{
    #region Constructor Injection

    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    public GetListByDynamicOrderQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    #endregion

    public async Task<GetListResponse<GetListByDynamicOrderListItemDto>> Handle(GetListByDynamicOrderQuery request, CancellationToken cancellationToken)
    {
        Paginate<Order> orders = await _orderRepository
            .GetListByDynamicAsync(
            dynamic: request.DynamicQuery!,
            pageNumber: request.PageRequest?.PageNumber ?? 0,
            pageSize: request.PageRequest?.PageSize ?? 10,
            withDeleted: true,
            cancellationToken: cancellationToken);

        GetListResponse<GetListByDynamicOrderListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicOrderListItemDto>>(orders);
        return response;
    }
}
