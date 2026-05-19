using Application.Pipelines.Caching.Remove;
using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Stocks.Commands.Delete;

public class DeleteStockCommand : IRequest<DeletedStockResponse>, ITransactionalRequest, ICacheRemoverRequest
{
    public Guid Id { get; set; }


    public string? CacheKey => null;
    public string? CacheGroupKey => "GetStocksQuery";
    public bool BypassCache => false;
}
