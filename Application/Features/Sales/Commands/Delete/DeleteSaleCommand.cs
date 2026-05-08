using MediatR;

namespace Application.Features.Sales.Commands.Delete;

public class DeleteSaleCommand : IRequest<DeletedSaleResponse>
{
    public Guid Id { get; set; }
}
