using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Suppliers.Commands.Delete;

public class DeleteSupplierCommand : IRequest<DeletedSupplierResponse>, ITransactionalRequest
{
    public Guid Id { get; set; }
}
