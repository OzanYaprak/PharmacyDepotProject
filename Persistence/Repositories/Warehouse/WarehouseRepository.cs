using Persistence.Contexts;

namespace Persistence.Repositories.Warehouse;

public class WarehouseRepository : EntityFrameworkRepositoryBase<Domain.Entities.Warehouse, Guid, BaseDbContext>, IWarehouseRepository
{
    public WarehouseRepository(BaseDbContext dbContext) : base(dbContext)
    {
    }
}
