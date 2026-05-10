using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories.Order;

namespace Application.Features.Orders.Queries.GetById;

public class GetByIdOrderQueryHandler : IRequestHandler<GetByIdOrderQuery, GetByIdOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetByIdOrderQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<GetByIdOrderResponse> Handle(GetByIdOrderQuery request, CancellationToken cancellationToken)
    {
        Order? order = await _orderRepository.GetAsync(
            include: o => o.Include(x => x.Supplier).Include(x => x.OrderItems),
            predicate: o => o.Id == request.Id,
            cancellationToken: cancellationToken);

        if (order is null)
            throw new KeyNotFoundException($"Order with id '{request.Id}' was not found.");

        return _mapper.Map<GetByIdOrderResponse>(order);
    }
}
