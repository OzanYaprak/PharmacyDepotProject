using Domain.Entities.Enums;

namespace Application.Features.Orders.Queries.GetList;

public class GetListOrderListItemDto
{
    public Guid Id { get; set; }
    public string SupplierName { get; set; } = default!;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}
