namespace Application.Features.Sales.Queries.GetListByDynamic;

public class GetListByDynamicSaleListItemDto
{
    public Guid Id { get; set; }
    public string? CustomerName { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
}
