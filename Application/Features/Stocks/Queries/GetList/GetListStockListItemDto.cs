namespace Application.Features.Stocks.Queries.GetList;

public class GetListStockListItemDto
{
    public Guid Id { get; set; }
    public Guid DrugId { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
