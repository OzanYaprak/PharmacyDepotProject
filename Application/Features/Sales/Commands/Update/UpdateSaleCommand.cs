using MediatR;

namespace Application.Features.Sales.Commands.Update;

public class UpdateSaleCommand : IRequest<UpdatedSaleResponse>
{
    public Guid Id { get; set; }
    public decimal? TotalAmount { get; set; }
}
