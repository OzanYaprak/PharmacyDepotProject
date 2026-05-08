using MediatR;

namespace Application.Features.Stocks.Queries.GetById;

public class GetByIdStockQuery : IRequest<GetByIdStockResponse>
{
    public Guid Id { get; set; }
}
