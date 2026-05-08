using MediatR;

namespace Application.Features.Warehouses.Commands.Update;

public class UpdateWarehouseCommand : IRequest<UpdatedWarehouseResponse>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Location { get; set; }
    public int? Capacity { get; set; }
}
