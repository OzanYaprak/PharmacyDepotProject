using Domain.Entities.Enums;

namespace Application.Features.Orders.Queries.GetList;

public class GetListOrderListItemDto
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
}
