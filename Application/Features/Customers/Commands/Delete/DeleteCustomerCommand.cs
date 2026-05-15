using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Customers.Commands.Delete;

public class DeleteCustomerCommand : IRequest<DeletedCustomerResponse>, ITransactionalRequest
{
    public Guid Id { get; set; }
}
