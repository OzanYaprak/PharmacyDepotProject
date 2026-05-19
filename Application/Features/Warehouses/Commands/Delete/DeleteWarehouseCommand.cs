using Application.Pipelines.Caching.Remove;
using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Warehouses.Commands.Delete;

public class DeleteWarehouseCommand : IRequest<DeletedWarehouseResponse>, ITransactionalRequest, ICacheRemoverRequest
{
    public Guid Id { get; set; }


    public string? CacheKey => null;
    public string? CacheGroupKey => "GetWarehousesQuery";
    public bool BypassCache => false;
}
