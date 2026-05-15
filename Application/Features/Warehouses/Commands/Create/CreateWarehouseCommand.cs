using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Warehouses.Commands.Create;

public class CreateWarehouseCommand : IRequest<CreatedWarehouseResponse>, ITransactionalRequest
{
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public int Capacity { get; set; }
}
