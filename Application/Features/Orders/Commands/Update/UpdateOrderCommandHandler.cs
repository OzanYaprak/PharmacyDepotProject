using Application.Features.Orders.Rules;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Order;

namespace Application.Features.Orders.Commands.Update;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, UpdatedOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly OrderBusinessRules _orderBusinessRules;

    public UpdateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, OrderBusinessRules orderBusinessRules)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _orderBusinessRules = orderBusinessRules;
    }

    public async Task<UpdatedOrderResponse> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        await _orderBusinessRules.CancelledOrderCannotBeUpdated(request.Id);
        await _orderBusinessRules.DeliveredOrderCannotBeUpdated(request.Id);

        Order? order = await _orderRepository.GetAsync(
            predicate: o => o.Id == request.Id,
            cancellationToken: cancellationToken);

        if (order is null)
            throw new KeyNotFoundException($"Order with id '{request.Id}' was not found.");

        _mapper.Map(request, order);

        var result = await _orderRepository.UpdateAsync(order, cancellationToken);
        return _mapper.Map<UpdatedOrderResponse>(result);
    }
}
