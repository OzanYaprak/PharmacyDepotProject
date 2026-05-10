using Domain.Entities.Enums;

namespace Application.Features.Orders.Queries.GetById;

public class GetByIdOrderResponse
{
    public Guid Id { get; set; }
    public string? SupplierName { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}
