using Application.Pipelines.Caching.Remove;
using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Stocks.Commands.Update;

public class UpdateStockCommand : IRequest<UpdatedStockResponse>, ITransactionalRequest, ICacheRemoverRequest
{
    public Guid Id { get; set; }
    public int? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }


    public string? CacheKey => null;
    public string? CacheGroupKey => "GetStocksQuery";
    public bool BypassCache => false;
}
