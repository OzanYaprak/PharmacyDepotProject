namespace Persistence.Repositories.Sale;

using Domain.Entities;
using Persistence.Contexts;

public class SaleRepository : EntityFrameworkRepositoryBase<Sale, Guid, BaseDbContext>, ISaleRepository
{
    public SaleRepository(BaseDbContext dbContext) : base(dbContext)
    {
    }
}
