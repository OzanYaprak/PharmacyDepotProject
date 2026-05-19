using Application.Pipelines.Caching.Remove;
using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Sales.Commands.Create;

public class CreateSaleCommand : IRequest<CreatedSaleResponse>, ITransactionalRequest, ICacheRemoverRequest
{
    public Guid CustomerId { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }


    public string? CacheKey => null;
    public string? CacheGroupKey => "GetSalesQuery";
    public bool BypassCache => false;
}
