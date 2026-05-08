using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.Sale;

/// <summary>
/// Sale entity'si için özel repository arayüzü.
/// </summary>
public interface ISaleRepository : IAsyncRepository<Domain.Entities.Sale, Guid>
{
}
