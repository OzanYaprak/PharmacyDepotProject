namespace Application.Features.Sales.Queries.GetList;

public class GetListSaleListItemDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
}
