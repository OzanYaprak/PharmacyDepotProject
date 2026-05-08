using MediatR;

namespace Application.Features.Sales.Commands.Create;

public class CreateSaleCommand : IRequest<CreatedSaleResponse>
{
    public Guid CustomerId { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
}
