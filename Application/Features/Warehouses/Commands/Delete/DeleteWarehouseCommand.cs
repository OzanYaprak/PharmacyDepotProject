using MediatR;

namespace Application.Features.Warehouses.Commands.Delete;

public class DeleteWarehouseCommand : IRequest<DeletedWarehouseResponse>
{
    public Guid Id { get; set; }
}
