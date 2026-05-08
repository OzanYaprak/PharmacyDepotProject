using MediatR;

namespace Application.Features.Customers.Commands.Delete;

public class DeleteCustomerCommand : IRequest<DeletedCustomerResponse>
{
    public Guid Id { get; set; }
}
