namespace Application.Features.Stocks.Queries.GetListByDynamic;

public class GetListByDynamicStockListItemDto
{
    public Guid Id { get; set; }
    public string? DrugName { get; set; }
    public string? WarehouseName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
