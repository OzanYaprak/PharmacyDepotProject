namespace Application.Features.Warehouses.Queries.GetListByDynamic;

public class GetListByDynamicWarehouseListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public int Capacity { get; set; }
}
