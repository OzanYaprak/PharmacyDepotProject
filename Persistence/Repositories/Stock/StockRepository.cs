using Persistence.Contexts;

namespace Persistence.Repositories.Stock;

public class StockRepository : EntityFrameworkRepositoryBase<Domain.Entities.Stock, Guid, BaseDbContext>, IStockRepository
{
    public StockRepository(BaseDbContext dbContext) : base(dbContext)
    {
    }
}
