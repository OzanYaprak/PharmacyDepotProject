namespace Application.Features.Warehouses.Commands.Update;

public class UpdatedWarehouseResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Location { get; set; }
    public int Capacity { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
