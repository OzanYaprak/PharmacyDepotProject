using MediatR;

namespace Application.Features.Stocks.Commands.Update;

public class UpdateStockCommand : IRequest<UpdatedStockResponse>
{
    public Guid Id { get; set; }
    public int? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
}
