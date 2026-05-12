using Application.Features.Orders.Rules;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Order;

namespace Application.Features.Orders.Commands.Create;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreatedOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly OrderBusinessRules _orderBusinessRules;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, OrderBusinessRules orderBusinessRules)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _orderBusinessRules = orderBusinessRules;
    }

    public async Task<CreatedOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        await _orderBusinessRules.OrderDateCannotBeInTheFuture(request.OrderDate);

        Order order = _mapper.Map<Order>(request);
        order.Id = Guid.NewGuid();

        var result = await _orderRepository.AddAsync(order, cancellationToken);
        return _mapper.Map<CreatedOrderResponse>(result);
    }
}
