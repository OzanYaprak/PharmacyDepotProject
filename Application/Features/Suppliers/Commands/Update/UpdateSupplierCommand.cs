using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Suppliers.Commands.Update;

public class UpdateSupplierCommand : IRequest<UpdatedSupplierResponse>, ITransactionalRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}
