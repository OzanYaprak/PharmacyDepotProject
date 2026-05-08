using MediatR;

namespace Application.Features.Orders.Commands.Delete;

public class DeleteOrderCommand : IRequest<DeletedOrderResponse>
{
    public Guid Id { get; set; }
}
