using Persistence.Contexts;

namespace Persistence.Repositories.SaleItem;

public class SaleItemRepository : EntityFrameworkRepositoryBase<Domain.Entities.SaleItem, Guid, BaseDbContext>, ISaleItemRepository
{
    public SaleItemRepository(BaseDbContext dbContext) : base(dbContext)
    {
    }
}
