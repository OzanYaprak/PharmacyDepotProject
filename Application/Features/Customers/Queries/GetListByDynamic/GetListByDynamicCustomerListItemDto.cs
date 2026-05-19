namespace Application.Features.Customers.Queries.GetListByDynamic;

public class GetListByDynamicCustomerListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string LicenseNumber { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Address { get; set; } = default!;
}
