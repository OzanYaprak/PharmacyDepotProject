using MediatR;

namespace Application.Features.Suppliers.Commands.Delete;

public class DeleteSupplierCommand : IRequest<DeletedSupplierResponse>
{
    public Guid Id { get; set; }
}
