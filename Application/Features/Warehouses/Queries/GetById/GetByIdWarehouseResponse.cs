namespace Application.Features.Warehouses.Queries.GetById;

public class GetByIdWarehouseResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public int Capacity { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}
