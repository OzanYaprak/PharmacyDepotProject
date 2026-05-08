namespace Application.Features.Sales.Commands.Create;

public class CreatedSaleResponse
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedDate { get; set; }
}
