using MediatR;

namespace Application.Features.Suppliers.Commands.Create;

public class CreateSupplierCommand : IRequest<CreatedSupplierResponse>
{
    public string Name { get; set; } = default!;
    public string ContactPerson { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Address { get; set; } = default!;
}
