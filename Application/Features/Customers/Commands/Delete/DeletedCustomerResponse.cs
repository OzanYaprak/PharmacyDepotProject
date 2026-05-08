namespace Application.Features.Customers.Commands.Delete;

public class DeletedCustomerResponse
{
    public Guid Id { get; set; }
    public DateTime? DeletedDate { get; set; }
}
