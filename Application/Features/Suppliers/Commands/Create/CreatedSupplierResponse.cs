namespace Application.Features.Suppliers.Commands.Create;

public class CreatedSupplierResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string ContactPerson { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Address { get; set; } = default!;
    public DateTime CreatedDate { get; set; }
}
