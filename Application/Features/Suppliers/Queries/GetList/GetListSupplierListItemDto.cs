namespace Application.Features.Suppliers.Queries.GetList;

public class GetListSupplierListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string ContactPerson { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Address { get; set; } = default!;
}
