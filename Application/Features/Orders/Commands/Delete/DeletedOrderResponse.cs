namespace Application.Features.Orders.Commands.Delete;

public class DeletedOrderResponse
{
    public Guid Id { get; set; }
    public DateTime? DeletedDate { get; set; }
}
