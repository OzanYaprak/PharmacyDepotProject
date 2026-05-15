using Application.Pipelines.Transaction;
using Domain.Entities.Enums;
using MediatR;

namespace Application.Features.Orders.Commands.Update;

public class UpdateOrderCommand : IRequest<UpdatedOrderResponse>, ITransactionalRequest
{
    public Guid Id { get; set; }
    public OrderStatus? Status { get; set; }
}
