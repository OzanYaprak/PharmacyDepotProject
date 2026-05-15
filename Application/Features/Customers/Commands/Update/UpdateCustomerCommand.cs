using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Customers.Commands.Update;

public class UpdateCustomerCommand : IRequest<UpdatedCustomerResponse>, ITransactionalRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? LicenseNumber { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}
