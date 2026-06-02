using Persistence.Contexts;

namespace Persistence.Repositories.OperationClaim;

public class OperationClaimRepository : EntityFrameworkRepositoryBase<Security.Entities.OperationClaim, int, BaseDbContext>, IOperationClaimRepository
{
    public OperationClaimRepository(BaseDbContext dbContext) : base(dbContext)
    {
    }
}
