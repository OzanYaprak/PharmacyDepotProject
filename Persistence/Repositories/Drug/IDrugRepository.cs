using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.Drug;

/// <summary>
/// Drug entity'si için özel repository arayüzü.
/// </summary>
public interface IDrugRepository : IAsyncRepository<Domain.Entities.Drug, Guid>
{
}
