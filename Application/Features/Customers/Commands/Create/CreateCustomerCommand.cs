using MediatR;

namespace Application.Features.Customers.Commands.Create;

public class CreateCustomerCommand : IRequest<CreatedCustomerResponse>
{
    public string Name { get; set; } = default!;
    public string LicenseNumber { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Address { get; set; } = default!;
}
