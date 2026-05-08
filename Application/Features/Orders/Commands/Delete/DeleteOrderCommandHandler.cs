using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Order;

namespace Application.Features.Orders.Commands.Delete;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, DeletedOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public DeleteOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<DeletedOrderResponse> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        Order? order = await _orderRepository.GetAsync(
            predicate: o => o.Id == request.Id,
            cancellationToken: cancellationToken);

        if (order is null)
            throw new KeyNotFoundException($"Order with id '{request.Id}' was not found.");

        await _orderRepository.DeleteAsync(order, permanent: false, cancellationToken: cancellationToken);
        return _mapper.Map<DeletedOrderResponse>(order);
    }
}
