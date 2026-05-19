using Application.Pipelines.Caching.Remove;
using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Stocks.Commands.Create;

public class CreateStockCommand : IRequest<CreatedStockResponse>, ITransactionalRequest, ICacheRemoverRequest
{
    public Guid DrugId { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }


    public string? CacheKey => null;
    public string? CacheGroupKey => "GetStocksQuery";
    public bool BypassCache => false;
}
