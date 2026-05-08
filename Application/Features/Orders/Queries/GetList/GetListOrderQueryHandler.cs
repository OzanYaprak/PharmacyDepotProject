using Application.Common.Responses;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Paging;
using Persistence.Repositories.Order;

namespace Application.Features.Orders.Queries.GetList;

public class GetListOrderQueryHandler : IRequestHandler<GetListOrderQuery, GetListResponse<GetListOrderListItemDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetListOrderQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<GetListResponse<GetListOrderListItemDto>> Handle(GetListOrderQuery request, CancellationToken cancellationToken)
    {
        Paginate<Order> orders = await _orderRepository.GetListAsync(
            pageNumber: request.PageRequest?.PageNumber ?? 0,
            pageSize: request.PageRequest?.PageSize ?? 10,
            cancellationToken: cancellationToken);

        return _mapper.Map<GetListResponse<GetListOrderListItemDto>>(orders);
    }
}
