using Domain.Entities.Enums;
using MediatR;

namespace Application.Features.Orders.Commands.Create;

public class CreateOrderCommand : IRequest<CreatedOrderResponse>
{
    public Guid SupplierId { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}
