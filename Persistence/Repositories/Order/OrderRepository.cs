namespace Persistence.Repositories.Order;

using Domain.Entities;
using Persistence.Contexts;

public class OrderRepository : EntityFrameworkRepositoryBase<Order, Guid, BaseDbContext>, IOrderRepository
{
    public OrderRepository(BaseDbContext dbContext) : base(dbContext)
    {
    }
}
