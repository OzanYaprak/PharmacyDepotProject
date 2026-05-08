namespace Application.Features.Sales.Queries.GetById;

public class GetByIdSaleResponse
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}
