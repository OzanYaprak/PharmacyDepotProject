using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.UserOperationClaim;

public interface IUserOperationClaimRepository : IAsyncRepository<Security.Entities.UserOperationClaim, int>
{
    Task<IList<Security.Entities.OperationClaim>> GetOperationClaimsByUserIdAsync(int userId);
}
