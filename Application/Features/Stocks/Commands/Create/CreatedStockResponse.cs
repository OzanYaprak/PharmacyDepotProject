namespace Application.Features.Stocks.Commands.Create;

public class CreatedStockResponse
{
    public Guid Id { get; set; }
    public Guid DrugId { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime CreatedDate { get; set; }
}
