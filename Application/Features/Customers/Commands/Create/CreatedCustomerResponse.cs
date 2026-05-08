namespace Application.Features.Customers.Commands.Create;

public class CreatedCustomerResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string LicenseNumber { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Address { get; set; } = default!;
    public DateTime CreatedDate { get; set; }
}
