using Application.Pipelines.Caching.Remove;
using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Orders.Commands.Delete;

public class DeleteOrderCommand : IRequest<DeletedOrderResponse>, ITransactionalRequest, ICacheRemoverRequest
{
    public Guid Id { get; set; }


    public string? CacheKey => null;
    public string? CacheGroupKey => "GetOrdersQuery";
    public bool BypassCache => false;
}
