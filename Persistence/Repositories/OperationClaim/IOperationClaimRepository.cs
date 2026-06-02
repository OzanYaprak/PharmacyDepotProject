using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.OperationClaim;

public interface IOperationClaimRepository : IAsyncRepository<Security.Entities.OperationClaim, int>
{
}
