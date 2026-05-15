using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Stocks.Commands.Delete;

public class DeleteStockCommand : IRequest<DeletedStockResponse>, ITransactionalRequest
{
    public Guid Id { get; set; }
}
