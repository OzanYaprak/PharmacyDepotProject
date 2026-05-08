namespace Application.Features.Stocks.Commands.Update;

public class UpdatedStockResponse
{
    public Guid Id { get; set; }
    public Guid DrugId { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
