using MediatR;

namespace Application.Features.Orders.Queries.GetById;

public class GetByIdOrderQuery : IRequest<GetByIdOrderResponse>
{
    public Guid Id { get; set; }
}
