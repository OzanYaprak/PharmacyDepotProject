using Application.Pipelines.Caching.Remove;
using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Suppliers.Commands.Delete;

public class DeleteSupplierCommand : IRequest<DeletedSupplierResponse>, ITransactionalRequest, ICacheRemoverRequest
{
    public Guid Id { get; set; }


    public string? CacheKey => null;
    public string? CacheGroupKey => "GetSuppliersQuery";
    public bool BypassCache => false;
}
