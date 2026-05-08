using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Order;

namespace Application.Features.Orders.Commands.Update;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, UpdatedOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public UpdateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<UpdatedOrderResponse> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
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
