using Domain.Entities.Enums;

namespace Application.Features.Orders.Commands.Update;

public class UpdatedOrderResponse
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
