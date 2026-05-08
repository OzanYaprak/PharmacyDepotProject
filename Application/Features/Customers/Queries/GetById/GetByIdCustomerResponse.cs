namespace Application.Features.Customers.Queries.GetById;

public class GetByIdCustomerResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string LicenseNumber { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Address { get; set; } = default!;
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}
