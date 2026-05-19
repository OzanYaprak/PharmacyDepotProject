using Application.Pipelines.Caching.Remove;
using Application.Pipelines.Transaction;
using Domain.Entities.Enums;
using MediatR;

namespace Application.Features.Orders.Commands.Create;

public class CreateOrderCommand : IRequest<CreatedOrderResponse>, ITransactionalRequest, ICacheRemoverRequest
{
    public Guid SupplierId { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;


    public string? CacheKey => null;
    public string? CacheGroupKey => "GetOrdersQuery";
    public bool BypassCache => false;
}
