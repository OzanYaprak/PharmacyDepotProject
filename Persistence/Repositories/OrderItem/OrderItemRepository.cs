namespace Persistence.Repositories.OrderItem;

using Domain.Entities;
using Persistence.Contexts;

public class OrderItemRepository : EntityFrameworkRepositoryBase<OrderItem, Guid, BaseDbContext>, IOrderItemRepository
{
    public OrderItemRepository(BaseDbContext dbContext) : base(dbContext)
    {
    }
}
