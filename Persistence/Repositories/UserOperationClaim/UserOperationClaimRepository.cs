using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories.UserOperationClaim;

public class UserOperationClaimRepository : EntityFrameworkRepositoryBase<Security.Entities.UserOperationClaim, int, BaseDbContext>, IUserOperationClaimRepository
{
    public UserOperationClaimRepository(BaseDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IList<Security.Entities.OperationClaim>> GetOperationClaimsByUserIdAsync(int userId)
    {
        var operationClaims = await Query()
            .AsNoTracking()
            .Where(uoc => uoc.UserId == userId)
            .Select(uoc => new Security.Entities.OperationClaim { Id = uoc.OperationClaimId, Name = uoc.OperationClaim.Name })
            .ToListAsync();

        return operationClaims;
    }
}
