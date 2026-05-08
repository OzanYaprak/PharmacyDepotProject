using MediatR;

namespace Application.Features.Warehouses.Commands.Create;

public class CreateWarehouseCommand : IRequest<CreatedWarehouseResponse>
{
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public int Capacity { get; set; }
}
