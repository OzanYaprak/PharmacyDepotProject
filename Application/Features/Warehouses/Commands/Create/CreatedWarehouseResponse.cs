namespace Application.Features.Warehouses.Commands.Create;

public class CreatedWarehouseResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public int Capacity { get; set; }
    public DateTime CreatedDate { get; set; }
}
