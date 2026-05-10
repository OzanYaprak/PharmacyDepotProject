namespace Application.Features.Stocks.Queries.GetList;

public class GetListStockListItemDto
{
    public Guid Id { get; set; }
    public string? DrugName { get; set; }
    public string? WarehouseName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
