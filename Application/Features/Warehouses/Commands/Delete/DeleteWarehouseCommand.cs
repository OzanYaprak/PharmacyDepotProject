using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Warehouses.Commands.Delete;

public class DeleteWarehouseCommand : IRequest<DeletedWarehouseResponse>, ITransactionalRequest
{
    public Guid Id { get; set; }
}
