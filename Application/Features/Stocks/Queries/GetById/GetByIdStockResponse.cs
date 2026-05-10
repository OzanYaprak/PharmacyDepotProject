namespace Application.Features.Stocks.Queries.GetById;

public class GetByIdStockResponse
{
    public Guid Id { get; set; }
    public string? DrugName { get; set; }
    public string? WarehouseName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}
