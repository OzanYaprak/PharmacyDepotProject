using MediatR;

namespace Application.Features.Customers.Queries.GetById;

public class GetByIdCustomerQuery : IRequest<GetByIdCustomerResponse>
{
    public Guid Id { get; set; }
}
