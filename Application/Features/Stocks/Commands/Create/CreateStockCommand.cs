using MediatR;

namespace Application.Features.Stocks.Commands.Create;

public class CreateStockCommand : IRequest<CreatedStockResponse>
{
    public Guid DrugId { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
