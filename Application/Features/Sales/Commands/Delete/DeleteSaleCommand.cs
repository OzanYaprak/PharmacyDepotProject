using Application.Pipelines.Caching.Remove;
using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Sales.Commands.Delete;

public class DeleteSaleCommand : IRequest<DeletedSaleResponse>, ITransactionalRequest, ICacheRemoverRequest
{
    public Guid Id { get; set; }


    public string? CacheKey => null;
    public string? CacheGroupKey => "GetSalesQuery";
    public bool BypassCache => false;
}
