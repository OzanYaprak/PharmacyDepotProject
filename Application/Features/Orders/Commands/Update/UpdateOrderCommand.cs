using Application.Pipelines.Caching.Remove;
using Application.Pipelines.Transaction;
using Domain.Entities.Enums;
using MediatR;

namespace Application.Features.Orders.Commands.Update;

public class UpdateOrderCommand : IRequest<UpdatedOrderResponse>, ITransactionalRequest, ICacheRemoverRequest
{
    public Guid Id { get; set; }
    public OrderStatus? Status { get; set; }


    public string? CacheKey => null;
    public string? CacheGroupKey => "GetOrdersQuery";
    public bool BypassCache => false;
}
