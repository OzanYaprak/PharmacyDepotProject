using Application.Pipelines.Caching.Remove;
using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Warehouses.Commands.Create;

public class CreateWarehouseCommand : IRequest<CreatedWarehouseResponse>, ITransactionalRequest, ICacheRemoverRequest
{
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public int Capacity { get; set; }


    public string? CacheKey => null;
    public string? CacheGroupKey => "GetWarehousesQuery";
    public bool BypassCache => false;
}
