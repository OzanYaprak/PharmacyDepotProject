namespace Application.Features.Suppliers.Queries.GetListByDynamic;

public class GetListByDynamicSupplierListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string ContactPerson { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Address { get; set; } = default!;
}
