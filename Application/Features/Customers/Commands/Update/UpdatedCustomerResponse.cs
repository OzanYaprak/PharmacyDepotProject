namespace Application.Features.Customers.Commands.Update;

public class UpdatedCustomerResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? LicenseNumber { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
