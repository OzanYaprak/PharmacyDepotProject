namespace Application.Features.Warehouses.Queries.GetList;

public class GetListWarehouseListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public int Capacity { get; set; }
}
