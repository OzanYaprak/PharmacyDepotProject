namespace Application.Features.Stocks.Queries.GetById;

public class GetByIdStockResponse
{
    public Guid Id { get; set; }
    public Guid DrugId { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}
