using MediatR;

namespace Application.Features.Stocks.Commands.Delete;

public class DeleteStockCommand : IRequest<DeletedStockResponse>
{
    public Guid Id { get; set; }
}
