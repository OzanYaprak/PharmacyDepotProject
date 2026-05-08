using Persistence.Contexts;

namespace Persistence.Repositories.Supplier;

public class SupplierRepository : EntityFrameworkRepositoryBase<Domain.Entities.Supplier, Guid, BaseDbContext>, ISupplierRepository
{
    public SupplierRepository(BaseDbContext dbContext) : base(dbContext)
    {
    }
}
