using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Sales.Commands.Delete;

public class DeleteSaleCommand : IRequest<DeletedSaleResponse>, ITransactionalRequest
{
    public Guid Id { get; set; }
}
