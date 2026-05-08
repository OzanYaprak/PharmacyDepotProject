using MediatR;

namespace Application.Features.Sales.Queries.GetById;

public class GetByIdSaleQuery : IRequest<GetByIdSaleResponse>
{
    public Guid Id { get; set; }
}
