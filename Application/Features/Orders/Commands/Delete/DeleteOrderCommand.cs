using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Orders.Commands.Delete;

public class DeleteOrderCommand : IRequest<DeletedOrderResponse>, ITransactionalRequest
{
    public Guid Id { get; set; }
}
