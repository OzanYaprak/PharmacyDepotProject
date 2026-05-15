using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Sales.Commands.Update;

public class UpdateSaleCommand : IRequest<UpdatedSaleResponse>, ITransactionalRequest
{
    public Guid Id { get; set; }
    public decimal? TotalAmount { get; set; }
}
