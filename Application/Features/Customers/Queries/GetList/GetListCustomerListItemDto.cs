namespace Application.Features.Customers.Queries.GetList;

public class GetListCustomerListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string LicenseNumber { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Address { get; set; } = default!;
}
