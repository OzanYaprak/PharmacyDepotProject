using Application.Pipelines.Caching.Remove;
using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Sales.Commands.Update;

public class UpdateSaleCommand : IRequest<UpdatedSaleResponse>, ITransactionalRequest, ICacheRemoverRequest
{
    public Guid Id { get; set; }
    public decimal? TotalAmount { get; set; }


    public string? CacheKey => null;
    public string? CacheGroupKey => "GetSalesQuery";
    public bool BypassCache => false;
}
